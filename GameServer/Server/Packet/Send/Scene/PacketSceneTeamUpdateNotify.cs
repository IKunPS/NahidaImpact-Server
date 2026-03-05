using System;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneTeamUpdateNotify : BasePacket
{
    public PacketSceneTeamUpdateNotify(PlayerInstance player) : base(CmdIds.SceneTeamUpdateNotify)
    {
        var proto = new SceneTeamUpdateNotify
        {
            IsInMp = player.IsInMultiplayer()
        };
        
        var currentAvatarEntity = player.EntityAvatar;
        
        var sceneTeamAvatar = new SceneTeamAvatar
        {
            SceneEntityInfo = currentAvatarEntity.ToProto(),
            WeaponEntityId = player.WeaponEntityId,
            PlayerUid = (uint)player.Uid,
            WeaponGuid = currentAvatarEntity.AvatarInfo.WeaponGuid,
            EntityId = currentAvatarEntity.Id,
            AvatarGuid = currentAvatarEntity.AvatarInfo.Guid,
            AbilityControlBlock = currentAvatarEntity.GetAbilityControlBlock(),
            SceneId = player.SceneId,
        };
        
        // For multiplayer, set additional fields like Java version
        if (player.IsInMultiplayer())
        {
            sceneTeamAvatar.AvatarInfo = currentAvatarEntity.GetAvatarInfo();
            sceneTeamAvatar.SceneAvatarInfo = currentAvatarEntity.GetSceneAvatarInfo();
        }
        
        proto.SceneTeamAvatarList.Add(sceneTeamAvatar);

        SetData(proto);
    }
}