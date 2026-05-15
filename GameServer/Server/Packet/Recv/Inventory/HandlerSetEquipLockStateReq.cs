using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.SetEquipLockStateReq)]
public class HandlerSetEquipLockStateReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetEquipLockStateReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.SetEquipLockState(req.TargetEquipGuid, req.IsLocked);
        await Task.CompletedTask;
    }
}
