namespace NahidaImpact.GameServer.Game.Player.Team;

public class GameAvatarTeam
{
    public uint Index { get; set; }
    public List<ulong> AvatarGuidList { get; set; }

    public GameAvatarTeam()
    {
        AvatarGuidList = new();
    }
}
