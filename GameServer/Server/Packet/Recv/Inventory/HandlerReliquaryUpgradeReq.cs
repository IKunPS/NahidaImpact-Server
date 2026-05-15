using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.ReliquaryUpgradeReq)]
public class HandlerReliquaryUpgradeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ReliquaryUpgradeReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.UpgradeReliquary(
            req.TargetReliquaryGuid,
            req.FoodReliquaryGuidList.ToList(),
            req.ItemParamList.ToList());
        await Task.CompletedTask;
    }
}
