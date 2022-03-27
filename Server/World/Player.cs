namespace DiplomaServer.World;

public struct Player
{
    public int Id;
    public string HudConnectionId;

    public Player(int id, string hudConnectionId)
    {
        Id = id;
        HudConnectionId = hudConnectionId;
    }
}