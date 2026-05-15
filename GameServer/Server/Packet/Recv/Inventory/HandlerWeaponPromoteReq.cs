using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponPromoteReq)]
public class HandlerWeaponPromoteReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponPromoteReq.Parser.ParseFrom(data);
        connection.Player?.InventoryManager.PromoteWeapon(req.TargetWeaponGuid);
        await Task.CompletedTask;
    }
}
