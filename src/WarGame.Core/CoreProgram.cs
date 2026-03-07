using System.ComponentModel.Design;
using System.Drawing;
using System.Dynamic;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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
    private Dictionary<String, int> playerWins = new Dictionary<String, int>();
    public PlayerHand(int players)
    {
        for (int i = 0; i < players; i++)
        {
            playerhands[$"Player {i + 1}"] = new Hand();
            playerWins[$"Player {i + 1}"] = 0;
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
    public List<Hand> GetHands()
    {
        return playerhands.Values.ToList();
    }
    public void RemovePlayer(String player)
    {
        playerhands.Remove(player);
    }
    public void AddPlayer(String player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public void AddWin(String player)
    {
        playerWins[player] += 1;
    }
    public int GetPlayerWins(String player)
    {
        return playerWins[player];
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
            if (cards[i].rank > highestNum)
            {
                highestNum = cards[i].rank;
                highPlayers = [players[i]];
                winCards = [cards[i]];
            }
            else if (cards[i].rank == highestNum)
            {
                highPlayers.Add(players[i]);
                winCards.Add(cards[i]);
            }
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
    public void RemoveCard(Card card)
    {
        pot.Remove(card);
    }
    public void SetPot(List<Card> pot)
    {
        this.pot = pot;
    }
    public List<Card> GetPot()
    {
        return pot;
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
    private List<String> CheckPlayerCardsUnder(PlayerHand playerHand)
    {
        List<String> players = playerHand.GetPlayers();

        // Handle players without a card
        foreach (String player in playerHand.GetPlayers()) // Check if player doesnt has card and removes them from PlayerHand if they dont
        {
            if (playerHand.GetHand(player).GetCardCount() <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{player} has lost!");
                players.Remove(player);
                playerHand.RemovePlayer(player);
                Console.ResetColor();
            }
        }
        return players;
    }
    private List<String> CheckTiePlayers(List<String> players, List<Hand> hands)
    {
        for (int i = 0; i < hands.Count(); i++)
        {
            if (hands[i].GetCardCount() <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{players[i]} has lost!");
                players.Remove(players[i]);
                Console.ResetColor();
            }
        }
        return players;
    }
    private PlayedCards UpdatePlayedCard(List<String> players, List<Hand> hands)
    {
        PlayedCards playedCards = new PlayedCards();
        for (int i = 0; i < players.Count(); i++)
        {
            Card card = hands[i].GetCard();
            Console.Write($"{players[i]}: ");
            
            if (card.suite == "Hearts" || card.suite == "Diamonds")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write($"{card.rankPairs[card.rank]} of {card.suite}");
                Console.ResetColor();
                Console.WriteLine("");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write($"{card.rankPairs[card.rank]} of {card.suite}");
                Console.ResetColor();
                Console.WriteLine("");
            }
            playedCards.UpdateCard(players[i], card); // Add played card from each player from hand
        }
        return playedCards;
    }
    private void TieChecking((List<String>, List<Card>) tieChecker, PlayerHand playerHand, Pot pot)
    {
        if (tieChecker.Item1.Count() != 1) // Handle tie, if not, continue
        {
            Console.WriteLine("\nTie!");
            List<String> potRanks = new List<String>();

            foreach (Card card in pot.GetPot())
            {
                potRanks.Add(card.rankPairs[card.rank]);
            }

            Console.WriteLine(string.Format("Pot includes: {0}", string.Join(", ", potRanks)));
            PlayedCards tieCards;
            List<Hand> tieHands = new List<Hand>();

            foreach (String player in tieChecker.Item1) // Add hands of each player to tiehands list
            {
                tieHands.Add(playerHand.GetHand(player));
            }

            // Check for if the players can continue and if there was a tie
            tieChecker.Item1 = CheckTiePlayers(tieChecker.Item1, tieHands); 
            tieCards = UpdatePlayedCard(tieChecker.Item1, tieHands);

            // Add cards to pot
            foreach (Card card in tieCards.GetCards())
            {
                pot.AddCard(card);
            }

            (List<String>, List<Card>) tieCheckTie = tieCards.GetHighest();
            TieChecking(tieCheckTie, playerHand, pot);

            tieChecker = tieCheckTie;
            
            /** 
            for (int i = 0; i < tieChecker.Item1.Count(); i++) // Temp function to return cards back to owners after tie
            {
                Hand hand = playerHand.GetHand(tieChecker.Item1[i]);
                Card card = tieChecker.Item2[i];

                // Added card back to each player and then removes card from pot
                hand.AddCard(card);
                playerHand.UpdateHand(tieChecker.Item1[i], hand);
                pot.RemoveCard(card);
            }
            **/
        }
    }
    public bool WinConditionCheck(PlayerHand playerHand, int roundNumber)
    {
        // Win condition for game
        foreach (Hand hand in playerHand.GetHands())
        {
            if (hand.GetCardCount() >= 52)
                return true;
        }
        if (roundNumber >= 10000)
            return true;
        else
            return false;
    }
    public void PlayRound(Pot pot, PlayerHand playerHand)
    {

        // Handle players without cards 
        List<String> players;
        players = CheckPlayerCardsUnder(playerHand);

        // Player hands to played cards 
        PlayedCards playedCards;
        playedCards = UpdatePlayedCard(players, playerHand.GetHands());

        // Add cards to pot
        foreach (Card card in playedCards.GetCards())
        {
            pot.AddCard(card);
        }

        // Tie handling
        (List<String>, List<Card>) tieChecker = playedCards.GetHighest();

        TieChecking(tieChecker, playerHand, pot);

        // Round winner
        String winningPlayer = tieChecker.Item1[0]; // Get winning player
        Hand winHand = playerHand.GetHand(winningPlayer); // Get winning hand
        List<Card> winCards = pot.GetPot(); // Get pot from round

        Console.WriteLine("");
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write($"{winningPlayer} has won the round!");
        Console.ResetColor();
        Console.WriteLine("\n");

        playerHand.AddWin(winningPlayer); // Add a win from the player

        // Pot cards -> the back of the winners hand
        foreach (Card card in winCards)
        {
            winHand.AddCard(card);
        }
        playerHand.UpdateHand(winningPlayer, winHand);

        int cardTotals = 0; // DEBUG
        // Print out player cards and wins
        foreach (String player in playerHand.GetPlayers())
        {
            int cardCount = playerHand.GetHand(player).GetCardCount();
            cardTotals += cardCount; // DEBUG

            Console.Write($"\t\t{player} has {cardCount} cards with ");
            if (player == winningPlayer)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{playerHand.GetPlayerWins(player)} wins!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else
            {
                Console.Write($"{playerHand.GetPlayerWins(player)} wins!\n");
            }
        }
        Console.WriteLine($"\tCard totals: {cardTotals}");
    }
    public void DetermineWinner(PlayerHand playerHand)
    {
        List<String> players = playerHand.GetPlayers();
        int highestNum = 0;
        int pick = -1;

        for (int i = 0; i < playerHand.GetPlayers().Count(); i++)
        {
            int playerCardCount = playerHand.GetHand(players[i]).GetCardCount();
            if (playerCardCount >= 52)
                pick = i;
            if (playerCardCount > highestNum)
                highestNum = playerCardCount;
            else if (playerCardCount == highestNum)
                pick = -1;
        }
        if (pick == -1)
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("The game has ended in a tie!");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{players[pick]} has won the game!");
            Console.ResetColor();
        }
    }
}
