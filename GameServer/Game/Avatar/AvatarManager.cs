using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Player.Team;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using System.Linq;

namespace NahidaImpact.GameServer.Game.Avatar;

public class AvatarManager(PlayerInstance player) : BasePlayerManager(player)
{
    public AvatarData AvatarData { get; } = DatabaseHelper.GetInstanceOrCreateNew<AvatarData>(player.Uid);
    public async ValueTask<AvatarDataExcel?> AddAvatar(int avatarId)
    {
        if (AvatarData.Avatars.Any(a => a.AvatarId == avatarId)) return null;
        GameData.AvatarData.TryGetValue(avatarId, out var avatarExcel);
        if (avatarExcel == null) return null;

        uint currentTimestamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
        var avatar = new AvatarDataInfo
        {
            SkillDepotId = avatarExcel.SkillDepotId,
            AvatarId = avatarExcel.Id,
            Guid = NextGuid(),
            WeaponId = avatarExcel.InitialWeapon,
            BornTime = currentTimestamp,
            WearingFlycloakId = 340005
        };
        
        avatar.InitDefaultProps(avatarExcel);
        AvatarData.Avatars.Add(avatar);
        
        if (Player.TeamManager != null) Player.TeamManager.AddAvatarToCurrentTeam(avatar.Guid);

        return avatarExcel;
    }
    
    public async ValueTask<bool> InitializeDefaultAvatar()
    {
        if (AvatarData.Avatars.Count > 0)
            return false;
        
        var avatarExcel = await AddAvatar(10000007);
        return avatarExcel != null;
    }
    
    public GameAvatarTeam GetCurrentTeam()
    {
        if (Player.TeamManager != null) return Player.TeamManager.GetCurrentTeamInfo();

        return new GameAvatarTeam { Index = 1 };
    }

    public ulong NextGuid()
    {
        return ((ulong)Player.Uid << 32) + (++Player.GuidSeed);
    }

    public AvatarDataInfo? GetAvatar(int avatarId)
    {
        return AvatarData.Avatars.Find(avatar => avatar.AvatarId == avatarId);
    }

    public AvatarDataInfo? GetAvatarByGuid(ulong guid)
    {
        return AvatarData.Avatars.Find(avatar => avatar.Guid == guid);
    }
}