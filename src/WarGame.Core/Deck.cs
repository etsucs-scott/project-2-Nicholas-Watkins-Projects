namespace WarGame.Core;

/// <summary>
/// A deck of cards that inits with 52 cards, 4 suites and 13 ranks. 
/// </summary>
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