
namespace WarGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int playerAmount = 0;
            while (true) // INIT Players count by getting from player
            {
                Console.Clear();
                Console.WriteLine("How many players? (2-4): ");
                int.TryParse(Console.ReadLine(), out playerAmount);
                if (playerAmount >= 2 && playerAmount < 5)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Try again bub");
                }
            }
            
            PlayerHand playerHands = new PlayerHand(playerAmount);
            Deck deck = new Deck();
            Deal dealing = new Deal(); // Like FDR frfr
            deck.Shuffle();
            dealing.DealCards(playerHands, deck);

            Round gameRounds = new Round();

            bool winCondition = false;
            while (!winCondition)
            {
                Pot pot = new Pot();
                winCondition = gameRounds.RoundLoop(pot, playerHands);
            }
        }
    }
}
