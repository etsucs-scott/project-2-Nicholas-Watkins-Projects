
using System.Drawing;

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
                    Console.WriteLine("Try again bub\nPress enter to continue");
                    Console.ReadLine();
                }
            }
            
            PlayerHand playerHands = new PlayerHand(playerAmount);
            Deck deck = new Deck();
            Deal dealing = new Deal(); // Like FDR frfr
            deck.Shuffle();
            dealing.DealCards(playerHands, deck);

            Round gameRounds = new Round();

            bool winCondition = false;
            int roundNumber = 1;
            while (!winCondition)
            {
                Pot pot = new Pot();
                Console.WriteLine("\n");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write($"##### Round {roundNumber} #####");
                Console.ResetColor();
                Console.WriteLine("\n");

                gameRounds.PlayRound(pot, playerHands);
                winCondition = gameRounds.WinConditionCheck(playerHands, roundNumber);
                roundNumber += 1;
            }
            Console.WriteLine("WON GAME YIPPEE CONGRATULATIONS MY PRETTY");
        }
    }
}
