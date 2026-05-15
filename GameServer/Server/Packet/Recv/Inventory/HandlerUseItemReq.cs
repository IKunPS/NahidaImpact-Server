using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.UseItemReq)]
public class HandlerUseItemReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = UseItemReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.UseItem(req.Guid, req.Count, req.TargetGuid, req.OptionIdx);
        await Task.CompletedTask;
    }
}
