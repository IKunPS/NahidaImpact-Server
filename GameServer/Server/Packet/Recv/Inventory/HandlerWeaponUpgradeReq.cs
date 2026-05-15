using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponUpgradeReq)]
public class HandlerWeaponUpgradeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponUpgradeReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.UpgradeWeapon(
            req.TargetWeaponGuid,
            req.FoodWeaponGuidList.ToList(),
            req.ItemParamList.ToList());
        await Task.CompletedTask;
    }
}
