using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponUpgradeReq)]
public class HandlerWeaponUpgradeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponUpgradeReq.Parser.ParseFrom(data);
        var result = connection.Player?.WeaponManager.UpgradeWeapon(
            req.TargetWeaponGuid,
            req.FoodWeaponGuidList.ToList(),
            req.ItemParamList.ToList());

        if (result == null || !result.IsSuccess)
        {
            await connection.SendPacket(new BasePacket((ushort)CmdIds.WeaponUpgradeRsp));
            return;
        }

        await connection.SendPacket(new PacketWeaponUpgradeRsp(
            req.TargetWeaponGuid, result.OldLevel, result.NewLevel, result.ReturnItems));
    }
}