using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarChangeCostumeNotify : BasePacket
{
    public PacketAvatarChangeCostumeNotify(SceneEntityInfo entityInfo)
        : base(CmdIds.AvatarChangeCostumeNotify)
    {
        var proto = new AvatarChangeCostumeNotify
        {
            EntityInfo = entityInfo
        };
        SetData(proto);
    }
}
