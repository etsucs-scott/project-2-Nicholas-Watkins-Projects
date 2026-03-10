using System.ComponentModel.Design;
using System.Drawing;
using System.Dynamic;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WarGame.Core;

/// <summary>
/// Holds the logic of the round of the game
/// </summary>
public class Round
{
    public PlayerList CheckPlayerCardsUnder(PlayerHand playerHand) // Handles players that don't have a card 
    {
        List<string> players = playerHand.GetPlayers();
        PlayerList pList = new PlayerList(players);

        // Handle players without a card
        foreach (string player in playerHand.GetPlayers()) // Check if player doesnt has card and removes them from PlayerHand if they dont
        {
            if (playerHand.GetHand(player).GetCardCount() <= 0)
            {
                pList.consoleOutput = $"{player} has lost!\n";
                players.Remove(player);
                playerHand.RemovePlayer(player);
            }
        }
        return pList;
    }
    public PlayerList CheckTiePlayers(List<string> players, List<Hand> hands) // Also checks if players dont have a card but within a tie condition
    {
        PlayerList pList = new PlayerList(players);

        for (int i = 0; i < hands.Count(); i++)
        {
            if (hands[i].GetCardCount() <= 0)
            {
                pList.consoleOutput = $"{players[i]} has lost!\n";
                players.Remove(players[i]);
            }
        }
        return pList;
    }
    public PlayedCardList UpdatePlayedCard(List<string> players, List<Hand> hands) // Updates the played cards by getting a card from each player and displaying it
    {
        PlayedCards playedCards = new PlayedCards();
        PlayedCardList pList = new PlayedCardList();

        for (int i = 0; i < players.Count(); i++)
        {
            Card card = hands[i].GetCard();
            pList.consoleOutput += $"{players[i]}: {card.rankPairs[card.rank]} of {card.suite}\n";
            playedCards.UpdateCard(players[i], card); // Add played card from each player from hand
        }
        pList.pCards = playedCards;
        pList.isOut = true;

        return pList;
    }
    public PlayerCardList TieChecking((List<string>, List<Card>) tieChecker, PlayerHand playerHand, Pot pot, PlayerCardList playerCardList)
    {
        playerCardList.isOut = true;
        if (tieChecker.Item1.Count() != 1) // Handle tie, if not, continue
        {
            playerCardList.consoleOutput += "\nTie!\n";

            // Handles the pot includes text that displays 
            List<String> potRanks = new List<String>();
            foreach (Card card in pot.GetPot())
            {
                potRanks.Add(card.rankPairs[card.rank]);
            }
            playerCardList.consoleOutput += string.Format("Pot includes: {0}\n", string.Join(", ", potRanks));

            PlayedCards tieCards;
            List<Hand> tieHands = new List<Hand>();

            foreach (String player in tieChecker.Item1) // Add hands of each player to tiehands list
            {
                tieHands.Add(playerHand.GetHand(player));
            }

            // Check for if the players can continue and if there was a tie
            PlayerList pList = CheckTiePlayers(tieChecker.Item1, tieHands);
            playerCardList.consoleOutput += pList.consoleOutput;
            tieChecker.Item1 = pList.players;

            PlayedCardList pCardList = UpdatePlayedCard(tieChecker.Item1, tieHands);
            playerCardList.consoleOutput += pCardList.consoleOutput;
            tieCards = pCardList.pCards;

            // Add cards to pot
            foreach (Card card in tieCards.GetCards())
            {
                pot.AddCard(card);
            }

            // Rechecks new cards from tie round to see if its another tie
            playerCardList.playerCards = tieCards.GetHighest(); 
            playerCardList = TieChecking(playerCardList.playerCards, playerHand, pot, playerCardList);

            return playerCardList;
        }
        return playerCardList;
    }
    public bool WinConditionCheck(PlayerHand playerHand, int roundNumber)
    {
        // Win condition for game
        foreach (Hand hand in playerHand.GetHands()) // Checks if player hit 52 cards
        {
            if (hand.GetCardCount() >= 52)
                return true;
        }
        if (roundNumber >= 10000) // Checks if round is at 10000 or more
            return true;
        else
            return false;
    }
    public VoidList DetermineWinner(PlayerHand playerHand)
    {
        List<string> players = playerHand.GetPlayers();
        VoidList vList = new VoidList();
        int highestNum = 0;
        string pick = "";

        foreach (string player in players)
        {
            int playerCardCount = playerHand.GetHand(player).GetCardCount();

            if (playerCardCount > highestNum) // Sameish process from UpdatePlayedCards highest... Player is set if their card count is the highest
            {
                pick = player;
                highestNum = playerCardCount;
            }
            else if (playerCardCount == highestNum) // Defaults to tie but will set it to tie if any values match
                pick = "";
        }
        if (pick == "") // Tie handling
        {
            vList.consoleOutput += "The game has ended in a tie!";
        }
        else // Winner winner chicken dinner. That actaully makes me kinda hungry...
        {
            vList.consoleOutput += $"{pick} has won the game!";
        }
        vList.isOut = true;
        return vList;
    }
}