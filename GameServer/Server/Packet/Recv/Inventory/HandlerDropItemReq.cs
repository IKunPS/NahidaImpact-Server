using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.DropItemReq)]
public class HandlerDropItemReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = DropItemReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.RemoveItem(req.Guid, (int)req.Count);
        await Task.CompletedTask;
    }
}
