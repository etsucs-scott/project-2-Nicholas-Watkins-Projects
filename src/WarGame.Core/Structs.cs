namespace WarGame.Core;

/// <summary>
/// Has a List<string> object
/// </summary>
public struct PlayerList : StructOut
{
    public bool isOut { get; set; } = false;
    public string? consoleOutput { get; set; } = "";
    public List<string> players;
    public PlayerList(List<string> players, bool isOut = false, string consoleOutput = "")
    {
        this.isOut = isOut;
        this.players = players;
        this.consoleOutput = consoleOutput;
    }
}

/// <summary>
/// Has a PlayedCards object
/// </summary>
public struct PlayedCardList : StructOut
{
    public bool isOut { get; set; } = false;
    public string? consoleOutput { get; set; } = "";
    public PlayedCards pCards = new PlayedCards();
    public PlayedCardList(PlayedCards pCards, bool isOut = false, string consoleOutput = "")
    {
        this.isOut = isOut;
        this.pCards = pCards;
        this.consoleOutput = consoleOutput;
    }
}

/// <summary>
/// Has a (List<string>, List<Card>) object
/// </summary>
public struct PlayerCardList : StructOut
{
    public bool isOut { get; set; } = false;
    public string? consoleOutput { get; set; } = "";
    public (List<string>, List<Card>) playerCards;
    public PlayerCardList((List<string>, List<Card>) playerCards, bool isOut = false, string consoleOutput = "")
    {
        this.isOut = isOut;
        this.playerCards = playerCards;
        this.consoleOutput = consoleOutput;
    }
}

/// <summary>
/// No additional object
/// </summary>
public struct VoidList : StructOut
{
    public bool isOut { get; set; } = false;
    public string? consoleOutput { get; set; } = "";
    public VoidList(bool isOut = false, string consoleOutput = "")
    {
        this.isOut = isOut;
        this.consoleOutput = consoleOutput;
    }
}