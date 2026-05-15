using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.TakeoffEquipReq)]
public class HandlerTakeoffEquipReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = TakeoffEquipReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.UnequipItem(req.AvatarGuid, req.Slot);
        await Task.CompletedTask;
    }
}
