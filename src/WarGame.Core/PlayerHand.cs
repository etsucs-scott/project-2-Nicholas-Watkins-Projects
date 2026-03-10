namespace WarGame.Core;

/// <summary>
/// Player hand stores each players hand keyed to a string of their name
/// </summary>
public class PlayerHand
{
    private Dictionary<string, Hand> playerhands = new Dictionary<string, Hand>();
    private Dictionary<string, int> playerWins = new Dictionary<string, int>();
    public PlayerHand(int players) // init players
    {
        for (int i = 0; i < players; i++)
        {
            playerhands[$"Player {i + 1}"] = new Hand();
            playerWins[$"Player {i + 1}"] = 0;
        }
    }
    public Hand GetHand(string player)
    {
        return playerhands[player];
    }
    public void UpdateHand(string player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public List<string> GetPlayers()
    {
        return playerhands.Keys.ToList();
    }
    public List<Hand> GetHands()
    {
        return playerhands.Values.ToList();
    }
    public void RemovePlayer(string player)
    {
        playerhands.Remove(player);
    }
    public void AddPlayer(string player, Hand hand)
    {
        playerhands[player] = hand;
    }
    public void AddWin(string player)
    {
        playerWins[player] += 1;
    }
    public int GetPlayerWins(string player)
    {
        return playerWins[player];
    }
}