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
    private Stack<Card> deck = new Stack<Card>();
    public Deck()
    {
        // 52 card deck 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                deck.Push(new Card(i, j + 2));
            }
        }
    }
    public void Shuffle()
    {
        List<Card> list = deck.ToList();
        list = list.OrderBy(x => Random.Shared.Next()).ToList();
        deck = new Stack<Card>(list);
    }
    public void ShowDeck()
    {
        foreach (Card card in deck)
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