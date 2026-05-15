using NahidaImpact.Database;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Team;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Avatar;

public class AvatarManager(PlayerInstance player) : BasePlayerManager(player)
{
    public AvatarData AvatarData { get; } = DatabaseHelper.GetInstanceOrCreateNew<AvatarData>(player.Uid);

    public List<AvatarDataInfo> Avatars => AvatarData.Avatars;

    public int AvatarCount => Avatars.Count;

    public AvatarDataInfo? GetAvatar(int avatarId)
        => Avatars.Find(a => a.AvatarId == avatarId);

    public AvatarDataInfo? GetAvatarById(uint avatarId)
        => Avatars.Find(a => a.AvatarId == avatarId);

    public AvatarDataInfo? GetAvatarByGuid(ulong guid)
        => Avatars.Find(a => a.Guid == guid);

    public bool HasAvatar(int avatarId)
        => Avatars.Any(a => a.AvatarId == avatarId);

    public ulong NextGuid()
        => ((ulong)Player.Uid << 32) + (++Player.GuidSeed);

    public TeamInfo GetCurrentTeam()
    {
        if (Player.TeamManager != null)
            return Player.TeamManager.GetCurrentTeamInfo();
        return new TeamInfo { Index = 1 };
    }

    public void Save()
    {
        AvatarData.SaveAvatarData(AvatarData);
    }
}