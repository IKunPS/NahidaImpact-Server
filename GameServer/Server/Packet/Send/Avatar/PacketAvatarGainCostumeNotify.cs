using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarGainCostumeNotify : BasePacket
{
    public PacketAvatarGainCostumeNotify(uint costumeId)
        : base(CmdIds.AvatarGainCostumeNotify)
    {
        var proto = new AvatarGainCostumeNotify
        {
            CostumeId = costumeId
        };
        SetData(proto);
    }
}
