using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.ReliquaryDecomposeReq)]
public class HandlerReliquaryDecomposeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ReliquaryDecomposeReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.DecomposeReliquary(req.GuidList.ToList());
        await Task.CompletedTask;
    }
}
