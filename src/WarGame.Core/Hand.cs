namespace WarGame.Core;

/// <summary>
/// A hand for a player that holds card. Can get, add, and check card counts
/// </summary>
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