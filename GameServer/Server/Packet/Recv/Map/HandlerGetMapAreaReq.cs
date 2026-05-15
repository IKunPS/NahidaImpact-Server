using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

[Opcode(CmdIds.GetMapAreaReq)]
public class HandlerGetMapAreaReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(new PacketGetMapAreaRsp());
    }
}
