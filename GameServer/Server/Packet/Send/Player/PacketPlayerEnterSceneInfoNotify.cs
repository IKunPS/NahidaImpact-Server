using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerEnterSceneInfoNotify : BasePacket
{
    public PacketPlayerEnterSceneInfoNotify(PlayerInstance player) : base(CmdIds.PlayerEnterSceneInfoNotify)
    {
        var proto = new PlayerEnterSceneInfoNotify()
        {
            CurAvatarEntityId = player.SceneManager!.TeamAvatars[0].EntityId,
            EnterSceneToken = player.SceneManager.EnterToken,
            MpLevelEntityInfo = new MPLevelEntityInfo
            {
                EntityId = 184549377,
                AbilityInfo = new AbilitySyncStateInfo(),
                AuthorityPeerId = 1
            },
            TeamEnterInfo = new TeamEnterSceneInfo
            {
                TeamEntityId = 150994946,
                AbilityControlBlock = new AbilityControlBlock(),
                TeamAbilityInfo = new AbilitySyncStateInfo()
            }
        };
        
        proto.AvatarEnterInfo.Add(new AvatarEnterSceneInfo
        {
            AvatarGuid = player.EntityAvatar!.AvatarInfo.Guid,
            AvatarEntityId = player.EntityAvatar.EntityId,
            WeaponEntityId = player.WeaponEntityId,
            WeaponGuid = player.EntityAvatar.AvatarInfo.WeaponGuid
        });
        SetData(proto);
    }
}