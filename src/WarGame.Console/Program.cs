
using System.Drawing;

namespace WarGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int playerAmount = 0;
            while (true) // INIT Players count by getting from player/console input
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

            // init playerhands, deck, dealing, and then shuffle and deal cards 
            PlayerHand playerHands = new PlayerHand(playerAmount);
            Deck deck = new Deck();
            Deal dealing = new Deal(); // Like FDR frfr
            deck.Shuffle();
            dealing.DealCards(playerHands, deck);

            Round gameRounds = new Round();

            bool winCondition = false;
            int roundNumber = 1;
            while (!winCondition) // Game loop
            {
                Pot pot = new Pot();
                Console.WriteLine("\n");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write($"##### Round {roundNumber} #####");
                Console.ResetColor();
                Console.WriteLine("\n");


                PlayRound(gameRounds, pot, playerHands);
                winCondition = gameRounds.WinConditionCheck(playerHands, roundNumber);


                // Comment or delete to remove player prompt for game
                //Console.WriteLine("Press enter to continue");
                //Console.ReadLine();

                roundNumber += 1;
            }
            VoidList vList = gameRounds.DetermineWinner(playerHands);
            StructOutHandler(vList);
        }

        /// <summary>
        /// Play a round of the game
        /// </summary>
        /// <param name="gameRound">A Round object </param>
        /// <param name="pot">A pot of cards for the round</param>
        /// <param name="playerHand">Every players hand that is player</param>
        public static void PlayRound(Round gameRound, Pot pot, PlayerHand playerHand)
        {
            // Handle players without cards 
            PlayerList pList = gameRound.CheckPlayerCardsUnder(playerHand);
            List<string> players = pList.players;

            Console.ForegroundColor = ConsoleColor.Red;
            StructOutHandler(pList);
            Console.ResetColor();

            // Player hands to played cards 
            PlayedCards playedCards;
            PlayedCardList cList = gameRound.UpdatePlayedCard(players, playerHand.GetHands());
            playedCards = cList.pCards;
            StructOutHandler(cList);

            // Add cards to pot
            foreach (Card card in playedCards.GetCards())
            {
                pot.AddCard(card);
            }

            // Tie handling
            (List<String>, List<Card>) tieChecker = playedCards.GetHighest();

            PlayerCardList playerCardList = new PlayerCardList(playerCards: tieChecker);

            playerCardList = gameRound.TieChecking(tieChecker, playerHand, pot, playerCardList);
            StructOutHandler(playerCardList);
            tieChecker = playerCardList.playerCards;

            // Round winner
            String winningPlayer = tieChecker.Item1[0]; // Get winning player
            Hand winHand = playerHand.GetHand(winningPlayer); // Get winning hand
            List<Card> winCards = pot.GetPot(); // Get pot from round

            Console.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{winningPlayer} has won the round!");
            Console.ResetColor();
            Console.WriteLine("\n");

            playerHand.AddWin(winningPlayer); // Add a win from the player

            // Pot cards -> the back of the winners hand
            foreach (Card card in winCards)
            {
                winHand.AddCard(card);
            }
            playerHand.UpdateHand(winningPlayer, winHand);

            // Print out player cards and wins
            foreach (String player in playerHand.GetPlayers())
            {
                int cardCount = playerHand.GetHand(player).GetCardCount();

                Console.Write($"\t\t{player} has {cardCount} cards with ");
                if (player == winningPlayer)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write($"{playerHand.GetPlayerWins(player)} wins!");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.Write($"{playerHand.GetPlayerWins(player)} wins!\n");
                }
            }
        }

        /// <summary>
        /// To output from struct 
        /// </summary>
        /// <param name="structure">StructOut structure to aid in console out for Core</param>
        public static void StructOutHandler(StructOut structure)
        {
            if (structure.isOut)
            {
                Console.Write(structure.consoleOutput);
            }
        }
    }
}
