using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponAwakenReq)]
public class HandlerWeaponAwakenReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponAwakenReq.Parser.ParseFrom(data);
        var result = connection.Player?.WeaponManager.AwakenWeapon(
            req.TargetWeaponGuid,
            req.ItemGuidList.ToList());

        if (result == null || !result.IsSuccess)
        {
            await connection.SendPacket(new BasePacket((ushort)CmdIds.WeaponAwakenRsp));
            return;
        }

        await connection.SendPacket(new PacketWeaponAwakenRsp(
            req.TargetWeaponGuid, result.OldAffixMap, result.CurAffixMap, result.AwakenLevel));
    }
}