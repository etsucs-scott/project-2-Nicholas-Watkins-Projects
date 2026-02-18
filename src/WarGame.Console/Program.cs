
namespace WarGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Deck gameDeck = new Deck();
            gameDeck.Shuffle();
            gameDeck.ShowDeck();


            for (int i = 0; i < 10000; i++)
            {
                
            }
        }
    }
}
