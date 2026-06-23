using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarGainFlycloakNotify : BasePacket
{
    public PacketAvatarGainFlycloakNotify(uint flycloakId)
        : base(CmdIds.AvatarGainFlycloakNotify)
    {
        var proto = new AvatarGainFlycloakNotify
        {
            FlycloakId = flycloakId
        };
        SetData(proto);
    }
}
