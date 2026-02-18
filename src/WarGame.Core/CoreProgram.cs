using System.Dynamic;
using System.Net.Security;

namespace WarGame.Core;

// Placeholder classes to get the idea of the structure
public class Card
{
    public string suite { get; private set; }
    public int rank { get; private set; }
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
    public Stack<Card> cards { get; private set; } = new Stack<Card>();
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
    public void Shuffle()
    {
        List<Card> list = cards.ToList();
        list = list.OrderBy(x => Random.Shared.Next()).ToList();
        cards = new Stack<Card>(list);
    }
    public void ShowDeck()
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
public class Players
{
    // public Queue<Card> hand { get; private set; } // Should check if this is accepabled for hand rule
    public Dictionary<string, Queue<Card>> playerHands { get; private set; } = new Dictionary<string, Queue<Card>>();
    public Players(int playerCount) // Makes all player hands | Should add 4 player restriction #### NEEDED ####
    {
        for (int i = 0; i < playerCount; i++)
        {
            playerHands[$"Player {i + 1}"] =  new Queue<Card>();
        }
    }
    public void Deal(Deck deck) // Should deal cards in round robin order
    {
        deck.Shuffle();
        int i = 0;
        foreach (Card card in deck.cards)
        {
            if (i >= playerHands.Keys.Count())
                i = 0;

            Queue<Card> hand = playerHands[$"Player {i + 1}"];
            hand.Enqueue(card);

            i++; 
        }
    }
}
public class Game
{
    public List<Card> pot { get; private set; }
    public List<Card> tiePot { get; private set; }
    public void Round(Players players)
    {
        Console.WriteLine("TESTING...");
    }
}