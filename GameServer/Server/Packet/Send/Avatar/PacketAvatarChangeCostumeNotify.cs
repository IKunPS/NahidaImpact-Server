using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarChangeCostumeNotify : BasePacket
{
    // Full entity info for broadcast when the avatar is in the scene
    public PacketAvatarChangeCostumeNotify(SceneEntityInfo entityInfo)
        : base(CmdIds.AvatarChangeCostumeNotify)
    {
        var proto = new AvatarChangeCostumeNotify
        {
            EntityInfo = entityInfo
        };
        SetData(proto);
    }

    // Minimal notify for off-field avatars — only the SceneAvatarInfo the client needs
    public PacketAvatarChangeCostumeNotify(SceneAvatarInfo avatarInfo)
        : base(CmdIds.AvatarChangeCostumeNotify)
    {
        var proto = new AvatarChangeCostumeNotify
        {
            EntityInfo = new SceneEntityInfo
            {
                Avatar = avatarInfo
            }
        };
        SetData(proto);
    }
}
