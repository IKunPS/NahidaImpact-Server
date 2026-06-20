using System.Collections.Generic;
using System.Linq;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneTeamUpdateNotify : BasePacket
{
    // Hexenzirkel avatar IDs
    private static readonly HashSet<int> HexenzirkelAvatars = new()
    {
        10000123, 10000020, 10000022, 10000029,
        10000031, 10000038, 10000041, 10000043
    };

    // MoonPhase avatar IDs
    private static readonly HashSet<int> MoonAvatars = new()
    {
        10000122, 10000120, 10000119, 10000121, 10000116
    };

    public PacketSceneTeamUpdateNotify(PlayerInstance player) : base(CmdIds.SceneTeamUpdateNotify)
    {
        var proto = new SceneTeamUpdateNotify
        {
            IsInMp = player.IsInMultiplayer()
        };

        if (player.World != null)
        {
            foreach (var p in player.World.GetPlayers())
            {
                int hexenzirkelCount = 0;
                int moonPhaseCount = 0;

                var activeTeam = p.TeamManager?.GetActiveTeam(true);

                var currentEntity = p.TeamManager?.GetCurrentAvatarEntity();

                foreach (var entityAvatar in activeTeam)
                {
                    if (entityAvatar == null) continue;

                    int avatarId = (int)entityAvatar.AvatarInfo.AvatarId;
                    if (HexenzirkelAvatars.Contains(avatarId))
                        hexenzirkelCount++;
                    if (MoonAvatars.Contains(avatarId))
                        moonPhaseCount++;

                    bool isCurrent = entityAvatar == currentEntity;

                    var sceneTeamAvatar = new SceneTeamAvatar
                    {
                        PlayerUid = (uint)p.Uid,
                        AvatarGuid = entityAvatar.AvatarInfo.Guid,
                        SceneId = p.SceneId,
                        EntityId = entityAvatar.Id,
                        SceneEntityInfo = entityAvatar.ToProto(),
                        WeaponGuid = entityAvatar.AvatarInfo.WeaponGuid,
                        WeaponEntityId = entityAvatar.WeaponEntityId,
                        AbilityControlBlock = entityAvatar.GetAbilityControlBlock(),
                        IsPlayerCurAvatar = isCurrent,
                        IsOnScene = isCurrent,
                        AvatarAbilityInfo = new AbilitySyncStateInfo(),
                        WeaponAbilityInfo = new AbilitySyncStateInfo()
                    };

                    proto.SceneTeamAvatarList.Add(sceneTeamAvatar);
                }

                // Send Hexenzirkel/MoonPhase global value changes (mirrors Java)
                var entity = p.TeamManager?.Entity;
                if (entity != null)
                {
                    _ = p.SendPacket(new PacketServerGlobalValueChangeNotify(
                        entity.Id,
                        "SGV_HexenzirkelLevel",
                        hexenzirkelCount
                    ));
                    _ = p.SendPacket(new PacketServerGlobalValueChangeNotify(
                        entity.Id,
                        "SGV_MoonPhaseLevel",
                        moonPhaseCount
                    ));
                }
            }
        }

        SetData(proto);
    }
}
