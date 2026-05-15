using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WearEquipReq)]
public class HandlerWearEquipReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WearEquipReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.EquipItem(req.AvatarGuid, req.EquipGuid);
        await Task.CompletedTask;
    }
}
