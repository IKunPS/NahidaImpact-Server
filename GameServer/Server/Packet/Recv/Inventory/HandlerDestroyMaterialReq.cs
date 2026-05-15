using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.DestroyMaterialReq)]
public class HandlerDestroyMaterialReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = DestroyMaterialReq.Parser.ParseFrom(data);
        foreach (var mat in req.MaterialList)
            connection.Player?.InventoryManager.DestroyMaterial(mat.Guid, (int)mat.Count);
        await Task.CompletedTask;
    }
}
