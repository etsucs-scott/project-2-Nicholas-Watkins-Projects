namespace WarGame.Core;

/// <summary>
/// A card with a suite, rank, and rankPair that gives the string of the rank
/// </summary>
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