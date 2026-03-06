using System.ComponentModel.Design;
using System.Dynamic;
using System.Net.Quic;
using System.Net.Security;

namespace WarGame.Core;

public class Card
{
    public string suite { get; private set; }
    public int rank { get; private set; }
    public int value { get; private set; }
    public Dictionary<int, string> rankPairs { get; private set; } = new Dictionary<int, string>();
    public Card(int suitePick, int rankNum)
    {
        string[] suites = { "Clubs", "Diamonds", "Spades", "Hearts" }; // 0, 1, 2, 3
        if (rankNum >= 11)
        {
            string[] uniqueRank = { "J", "Q", "K", "A"};
            rankPairs[rankNum] = uniqueRank[rankNum - 11];
        }
        else
            rankPairs[rankNum] = $"{rankNum}";

        suite = suites[suitePick];
        rank = rankNum;
    }
}
public class Deck
{
    private Stack<Card> cards = new Stack<Card>();
    public Deck()
    {
        // 52 card deck 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                cards.Push(new Card(i, j + 2));
            }
        }
    }
    public Card Draw()
    {
        return cards.Pop();
    }
    public void Shuffle()
    {
        List<Card> list = cards.ToList();
        list = list.OrderBy(x => Random.Shared.Next()).ToList();
        cards = new Stack<Card>(list);
    }
    public int GetLength()
    {
        return cards.Count;
    }
    public void ShowDeck() // Shows deck using different colors, mainly for debug and testing
    {
        foreach (Card card in cards)
        {
            if (card.suite == "Clubs" || card.suite == "Spades")
                Console.WriteLine($"Card \"{ card.rankPairs[card.rank]}\" of {card.suite}");
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Card \"{ card.rankPairs[card.rank]}\" of {card.suite}");
                Console.ResetColor();
            }
        }
    }
}
public class Hand
{
    private Queue<Card> hand = new Queue<Card>();
    public Card GetCard()
    {
        return hand.Dequeue();
    }
    public void AddCard(Card card)
    {
        hand.Enqueue(card);
    }
    public int GetCardCount()
    {
        return hand.Count;
    }
}
public class PlayerHand
{
    private Dictionary<String, Hand> playerhands = new Dictionary<String, Hand>();
    public PlayerHand(int players)
    {
        for (int i = 0; i < players; i++)
        {
            playerhands[$"Player {i + 1}"] = new Hand();
        }
    }
    public Hand GetHand(String player)
    {
        return playerhands[player];
    }
    public void UpdateHand(String player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public List<String> GetPlayers()
    {
        return playerhands.Keys.ToList();
    }
    public void RemovePlayer(String player)
    {
        playerhands.Remove(player);
    }
    public void AddPlayer(String player, Hand hand)
    {
        playerhands[player] = hand;
    }
}
public class PlayedCards
{
    private Dictionary<String, Card> playedCards = new Dictionary<String, Card>();
    public Card GetCard(String player)
    {
        return playedCards[player];
    }
    public List<Card> GetCards()
    {
        return playedCards.Values.ToList();
    }
    public void UpdateCard(String player, Card card)
    {
        playedCards[player] = card;
    }
    public (List<String>, List<Card>) GetHighest()
    {
        int highestNum = 0;
        List<Card> winCards = new List<Card>();
        List<String> highPlayers = new List<String>();

        List<String> players = playedCards.Keys.ToList();
        List<Card> cards = playedCards.Values.ToList();

        // Find the highest ranked players and track them for tie handling
        for (int i = 0; i < playedCards.Count(); i++)
        {
            if (cards[i].value > highestNum)
            {
                highestNum = cards[i].rank;
                highPlayers = [players[i]];
                winCards = [cards[i]];
            }
            else if (cards[i].value == highestNum)
            {
                highPlayers.Add(players[i]);
                winCards.Add(cards[i]);
            }
        }
        // Remove tied players/winner
        foreach (String player in highPlayers)
        {
            playedCards.Remove(player);
        }

        return (highPlayers, winCards);
    }
}
public class Pot
{
    private List<Card> pot = new List<Card>();
    public void AddCard(Card card)
    {
        pot.Add(card);
    }
    public void SetPot(List<Card> pot)
    {
        this.pot = pot;
    }
}
public class TiePot
{
    private List<Card> tiePot = new List<Card>();
    public void AddCard(Card card)
    {
        tiePot.Add(card);
    }
    public void SetPot(List<Card> tiePot)
    {
        this.tiePot = tiePot;
    }
}
public class Deal
{
    public void DealCards(PlayerHand playerHand, Deck deck) // Deals card in round robin order
    {
        int playerChoice = 0;
        List<String> players = playerHand.GetPlayers();
        while (deck.GetLength() > 0)
        {
            if (playerChoice >= players.Count())
                playerChoice = 0;

            String player = players[playerChoice];

            Card card = deck.Draw();
            Hand hand = playerHand.GetHand(player);
            hand.AddCard(card);
            playerHand.UpdateHand(player, hand);
            playerChoice += 1;
        }
    }
}
public class Round
{
    private int roundNumber = 1;
    public bool RoundLoop(Pot pot, PlayerHand playerHand)
    {
        Console.WriteLine($"\nRound {roundNumber}");
        List<String> players = playerHand.GetPlayers();

        // Handle players without a card
        foreach (String player in players) // Check if player doesnt has card and removes them from PlayerHand if they dont
        {
            if (playerHand.GetHand(player).GetCardCount() <= 0)
            {
                playerHand.RemovePlayer(player);
                Console.WriteLine($"{player} has lost!");
            }
        }

        // Player hand cards to played cards 
        PlayedCards playedCards = new PlayedCards();
        for (int i = 0; i < players.Count(); i++)
        {
            Card card = playerHand.GetHand(players[0]).GetCard();
            playedCards.UpdateCard(players[0], card); // Add played card from each player from hand
        }

        // Add cards to pot
        (List<String>, List<Card>) tieChecker = playedCards.GetHighest();
        foreach (Card card in playedCards.GetCards())
        {
            pot.AddCard(card);
        }

        // Should be moved outside round loop....
        // Tie handling
        if (tieChecker.Item1.Count() != 1)
        {
            Console.WriteLine("HIT A TIE. Waiting...");
            for (int i = 1; i <= tieChecker.Item1.Count(); i++)
            {
                pot.AddCard(tieChecker.Item2[i]);
            }
            Console.ReadLine();
        }

        // Round winner
        String winningPlayer = tieChecker.Item1[0];
        Console.WriteLine($"{winningPlayer} has won the round!");
        foreach (String player in playerHand.GetPlayers())
        {
            Console.WriteLine($"\t\t{player} has {playerHand.GetHand(player).GetCardCount()} cards");
        }
        roundNumber += 1;

        if (playerHand.GetHand(winningPlayer).GetCardCount() >= 52 || playerHand.GetPlayers().Count() <= 1)
        {
            Console.WriteLine("WON GAME YIPPEE CONGRATULATIONS MY PRETTY");
            return true;
        }
        else if (roundNumber >= 10000)
        {
            Console.WriteLine("WON GAME YIPPEE CONGRATULATIONS MY PRETTY");
            return true;
        }
        else
        {
            return false;
        }
    }
}