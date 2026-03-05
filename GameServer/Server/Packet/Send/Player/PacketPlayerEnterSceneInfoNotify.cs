using System;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Linq;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerEnterSceneInfoNotify : BasePacket
{
    public PacketPlayerEnterSceneInfoNotify(PlayerInstance player) : base(CmdIds.PlayerEnterSceneInfoNotify)
    {
        var proto = new PlayerEnterSceneInfoNotify()
        {
            CurAvatarEntityId = player.TeamManager.GetCurrentAvatarEntity().Id,
            EnterSceneToken = player.EnterToken,
            MpLevelEntityInfo = new MPLevelEntityInfo
            {
                EntityId = player.World.getLevelEntityId(),
                AbilityInfo = new AbilitySyncStateInfo(),
                AuthorityPeerId = player.PeerId
            },
            TeamEnterInfo = new TeamEnterSceneInfo
            {
                TeamEntityId = player.TeamManager.Entity.Id,
                AbilityControlBlock = new AbilityControlBlock(),
                TeamAbilityInfo = new AbilitySyncStateInfo()
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
                    WeaponEntityId = player.WeaponEntityId,
                    WeaponGuid = avatarEntity.AvatarInfo.WeaponGuid,
                });
            }
        }
        
        SetData(proto);
    }
}