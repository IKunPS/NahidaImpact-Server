using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneTeamUpdateNotify : BasePacket
{
    public PacketSceneTeamUpdateNotify(PlayerInstance player) : base(CmdIds.SceneTeamUpdateNotify)
    {
        var proto = new SceneTeamUpdateNotify();
        
        proto.SceneTeamAvatarList.Add(new SceneTeamAvatar
        {
            SceneEntityInfo = player.EntityAvatar!.ToProto(),
            WeaponEntityId = player.WeaponEntityId,
            PlayerUid = (uint)player.Uid,
            WeaponGuid = player.EntityAvatar.AvatarInfo.WeaponGuid,
            EntityId = player.EntityAvatar.EntityId,
            AvatarGuid = player.EntityAvatar.AvatarInfo.Guid,
            AbilityControlBlock = player.EntityAvatar.BuildAbilityControlBlock(),
            SceneId = player.SceneId
        });
        
        SetData(proto);
    }
}