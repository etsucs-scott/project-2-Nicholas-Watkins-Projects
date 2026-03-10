namespace WarGame.Core;

/// <summary>
/// Pot of cards is all the cards from the round to be given to the winner
/// </summary>
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