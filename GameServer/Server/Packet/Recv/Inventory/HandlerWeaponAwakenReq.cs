using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponAwakenReq)]
public class HandlerWeaponAwakenReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponAwakenReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.AwakenWeapon(
            req.TargetWeaponGuid,
            req.ItemGuidList.ToList());
        await Task.CompletedTask;
    }
}
