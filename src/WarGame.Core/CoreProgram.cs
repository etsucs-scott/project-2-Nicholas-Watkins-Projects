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
    public Dictionary<int, string> rankPairs { get; private set; } = new Dictionary<int, string>();
    public Card(int suitePick, int rankNum)
    {
        string[] suites = { "Clubs", "Diamonds", "Spades", "Hearts" }; // 0, 1, 2, 3
        // Converts rankNum to store its string counterpart, including J-A
        if (rankNum >= 11)
        {
            string[] uniqueRank = { "J", "Q", "K", "A" };
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
        // 52 card deck init
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
    public void Shuffle() // Shuffle cards by randomly mixing a list then converting back to a stack
    {
        List<Card> list = cards.ToList();
        list = list.OrderBy(x => Random.Shared.Next()).ToList();
        cards = new Stack<Card>(list);
    }
    public int GetLength()
    {
        return cards.Count;
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
    private Dictionary<string, Hand> playerhands = new Dictionary<string, Hand>();
    private Dictionary<string, int> playerWins = new Dictionary<string, int>();
    public PlayerHand(int players) // init players
    {
        for (int i = 0; i < players; i++)
        {
            playerhands[$"Player {i + 1}"] = new Hand();
            playerWins[$"Player {i + 1}"] = 0;
        }
    }
    public Hand GetHand(string player)
    {
        return playerhands[player];
    }
    public void UpdateHand(string player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public List<string> GetPlayers()
    {
        return playerhands.Keys.ToList();
    }
    public List<Hand> GetHands()
    {
        return playerhands.Values.ToList();
    }
    public void RemovePlayer(string player)
    {
        playerhands.Remove(player);
    }
    public void AddPlayer(string player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public void AddWin(string player)
    {
        playerWins[player] += 1;
    }
    public int GetPlayerWins(string player)
    {
        return playerWins[player];
    }
}
public class PlayedCards
{
    private Dictionary<string, Card> playedCards = new Dictionary<string, Card>();
    public Card GetCard(string player)
    {
        return playedCards[player];
    }
    public List<Card> GetCards()
    {
        return playedCards.Values.ToList();
    }
    public void UpdateCard(string player, Card card)
    {
        playedCards[player] = card;
    }
    public (List<string>, List<Card>) GetHighest()
    {
        int highestNum = 0;
        List<Card> winCards = new List<Card>();
        List<String> highPlayers = new List<String>();

        List<String> players = playedCards.Keys.ToList();
        List<Card> cards = playedCards.Values.ToList();

        // Find the highest ranked players and track them for tie handling
        for (int i = 0; i < playedCards.Count(); i++)
        {
            if (cards[i].rank > highestNum) // If cards rank is highest, reset the highest players and the cards associated with it
            {
                highestNum = cards[i].rank;
                highPlayers = [players[i]];
                winCards = [cards[i]];
            } // If a card is equal to the highest so far, then add them with the group
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
    public void DealCards(PlayerHand playerHand, Deck deck) // Deals cards in round robin order
    {
        int playerChoice = 0;
        List<String> players = playerHand.GetPlayers();
        while (deck.GetLength() > 0)
        {
            if (playerChoice >= players.Count()) // Resets playerchoice to zero once over player count
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
    private List<string> CheckPlayerCardsUnder(PlayerHand playerHand) // Handles players that don't have a card 
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
    private List<string> CheckTiePlayers(List<string> players, List<Hand> hands) // Also checks if players dont have a card but within a tie condition
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
    private PlayedCards UpdatePlayedCard(List<string> players, List<Hand> hands) // Updates the played cards by getting a card from each player and displaying it
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
    private (List<string>, List<Card>) TieChecking((List<string>, List<Card>) tieChecker, PlayerHand playerHand, Pot pot)
    {
        if (tieChecker.Item1.Count() != 1) // Handle tie, if not, continue
        {
            Console.WriteLine("\nTie!");

            // Handles the pot includes text that displays 
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
            
            // Rechecks new cards from tie round to see if its another tie
            (List<String>, List<Card>) tieCheckTie = tieCards.GetHighest(); 
            tieCheckTie = TieChecking(tieCheckTie, playerHand, pot); 

            return tieCheckTie;
        }
        return tieChecker;
    }
    public bool WinConditionCheck(PlayerHand playerHand, int roundNumber)
    {
        // Win condition for game
        foreach (Hand hand in playerHand.GetHands()) // Checks if player hit 52 cards
        {
            if (hand.GetCardCount() >= 52)
                return true;
        }
        if (roundNumber >= 10000) // Checks if round is at 10000 or more
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

        tieChecker = TieChecking(tieChecker, playerHand, pot);

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

        // Print out player cards and wins
        foreach (String player in playerHand.GetPlayers())
        {
            int cardCount = playerHand.GetHand(player).GetCardCount();

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
    }
    public void DetermineWinner(PlayerHand playerHand)
    {
        List<string> players = playerHand.GetPlayers();
        int highestNum = 0;
        string pick = "";

        foreach (string player in players)
        {
            int playerCardCount = playerHand.GetHand(player).GetCardCount();

            if (playerCardCount > highestNum) // Sameish process from UpdatePlayedCards highest... Player is set if their card count is the highest
            {
                pick = player;
                highestNum = playerCardCount;
            }
            else if (playerCardCount == highestNum) // Defaults to tie but will set it to tie if any values match
                pick = "";
        }
        if (pick == "") // Tie handling
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("The game has ended in a tie!");
            Console.ResetColor();
        }
        else // Winner winner chicken dinner. That actaully makes me kinda hungry...
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{pick} has won the game!");
            Console.ResetColor();
        }
    }
}
