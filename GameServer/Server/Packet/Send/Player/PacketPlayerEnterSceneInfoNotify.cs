using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerEnterSceneInfoNotify : BasePacket
{
    public PacketPlayerEnterSceneInfoNotify(PlayerInstance player) : base(CmdIds.PlayerEnterSceneInfoNotify)
    {
        // Phlogiston scalar value — mirrors Java:
        // AbilityScalarValueEntry with key "SGV_PlayerTeam_Phlogiston"
        var scalarValue = new AbilityScalarValueEntry
        {
            FloatValue = player.GetPhlogistonValue()
        };
        scalarValue.Key = new AbilityString
        {
            Hash = Utils.AbilityHash("SGV_PlayerTeam_Phlogiston"),
            Str = "SGV_PlayerTeam_Phlogiston"
        };

        var phlogistonState = new AbilitySyncStateInfo();
        phlogistonState.SgvDynamicValueMap.Add(scalarValue);

        var currentAvatar = player.TeamManager.GetCurrentAvatarEntity();
        var teamEntity = player.TeamManager.Entity;

        var proto = new PlayerEnterSceneInfoNotify
        {
            CurAvatarEntityId = currentAvatar?.Id ?? 0,
            EnterSceneToken = player.EnterToken,
            MpLevelEntityInfo = new MPLevelEntityInfo
            {
                EntityId = player.World?.getLevelEntityId() ?? 0,
                AuthorityPeerId = player.World?.GetHostPeerId() ?? 0,
                AbilityInfo = phlogistonState
            },
            TeamEnterInfo = new TeamEnterSceneInfo
            {
                TeamEntityId = teamEntity?.Id ?? 0,
                TeamAbilityInfo = phlogistonState,
                AbilityControlBlock = player.TeamManager.GetAbilityControlBlock()
            }
        };

        var activeTeam = player.TeamManager.GetActiveTeam();
        if (activeTeam != null)
        {
            foreach (var avatarEntity in activeTeam)
            {
                proto.AvatarEnterInfo.Add(new AvatarEnterSceneInfo
                {
                    AvatarGuid = avatarEntity.AvatarInfo.Guid,
                    AvatarEntityId = avatarEntity.Id,
                    WeaponGuid = avatarEntity.AvatarInfo.WeaponGuid,
                    WeaponEntityId = avatarEntity.WeaponEntityId
                });
            }
        }

        SetData(proto);
    }
}
