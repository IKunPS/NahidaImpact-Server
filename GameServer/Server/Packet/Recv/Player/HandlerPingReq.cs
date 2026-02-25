using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PingReq)]
public class HandlerPingReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketPingRsp());
    }
}