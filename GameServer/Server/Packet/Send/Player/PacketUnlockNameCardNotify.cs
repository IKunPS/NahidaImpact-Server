using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketUnlockNameCardNotify : BasePacket
{
    public PacketUnlockNameCardNotify(uint nameCardId) : base(CmdIds.UnlockNameCardNotify)
    {
        var proto = new UnlockNameCardNotify
        {
            NameCardId = nameCardId
        };
        SetData(proto);
    }
}
