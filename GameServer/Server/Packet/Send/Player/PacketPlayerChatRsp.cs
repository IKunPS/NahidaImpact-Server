using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerChatRsp : BasePacket
{
    public PacketPlayerChatRsp() : base(CmdIds.PlayerChatRsp)
    {
        var proto = new PlayerChatRsp();
        SetData(proto);
    }
}