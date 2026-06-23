using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarFlycloakChangeNotify : BasePacket
{
    public PacketAvatarFlycloakChangeNotify(List<ulong> avatarGuidList, uint flycloakId)
        : base(CmdIds.AvatarFlycloakChangeNotify)
    {
        var proto = new AvatarFlycloakChangeNotify
        {
            FlycloakId = flycloakId
        };
        proto.AvatarGuidList.AddRange(avatarGuidList);
        SetData(proto);
    }
}
