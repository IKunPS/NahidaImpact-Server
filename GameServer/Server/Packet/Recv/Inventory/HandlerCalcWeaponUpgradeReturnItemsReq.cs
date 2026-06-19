using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.CalcWeaponUpgradeReturnItemsReq)]
public class HandlerCalcWeaponUpgradeReturnItemsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = CalcWeaponUpgradeReturnItemsReq.Parser.ParseFrom(data);
        var result = connection.Player?.WeaponManager.CalcWeaponUpgradeReturnItems(
            req.TargetWeaponGuid,
            req.FoodWeaponGuidList.ToList(),
            req.ItemParamList.ToList());

        if (result == null || !result.IsSuccess)
        {
            await connection.SendPacket(new BasePacket((ushort)CmdIds.CalcWeaponUpgradeReturnItemsRsp));
            return;
        }

        await connection.SendPacket(new PacketCalcWeaponUpgradeReturnItemsRsp(req.TargetWeaponGuid, result.ReturnItems));
    }
}