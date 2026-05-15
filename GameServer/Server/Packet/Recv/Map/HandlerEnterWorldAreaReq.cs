using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Map;

/// <summary>
/// Handles EnterWorldAreaReq - player entering a world area.
/// Ported from Java HandlerEnterWorldAreaReq.
/// </summary>
[Opcode(CmdIds.EnterWorldAreaReq)]
public class HandlerEnterWorldAreaReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = EnterWorldAreaReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        player.SetArea((int)req.AreaId, (int)req.AreaType);
        await connection.SendPacket(new PacketEnterWorldAreaRsp((int)req.AreaId, (int)req.AreaType));
    }
}