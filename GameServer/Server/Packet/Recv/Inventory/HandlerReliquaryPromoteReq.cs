using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.ReliquaryPromoteReq)]
public class HandlerReliquaryPromoteReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ReliquaryPromoteReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.PromoteReliquary(req.TargetGuid);
        await Task.CompletedTask;
    }
}
