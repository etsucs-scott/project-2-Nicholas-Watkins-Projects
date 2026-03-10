namespace WarGame.Core;

/// <summary>
/// A dictionary of cards played by each player in a round
/// </summary>
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
