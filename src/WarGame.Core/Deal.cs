namespace WarGame.Core;

/// <summary>
/// Deals the cards in round robin order
/// </summary>
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