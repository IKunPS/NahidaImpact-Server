using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Time;

public class PacketServerTimeNotify : BasePacket
{
    public PacketServerTimeNotify() : base(CmdIds.ServerTimeNotify)
    {
        var proto = new ServerTimeNotify
        {
            ServerTime = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };
        
        SetData(proto);
    }
}