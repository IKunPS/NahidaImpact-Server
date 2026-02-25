using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPingRsp : BasePacket
{
    public PacketPingRsp() : base(CmdIds.PingRsp)
    {
        var proto = new PingRsp()
        {
            ClientTime = (uint)DateTimeOffset.Now.ToUnixTimeSeconds()
        };
        
        SetData(proto);
    }
}