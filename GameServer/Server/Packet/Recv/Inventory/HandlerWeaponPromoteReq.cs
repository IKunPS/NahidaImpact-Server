using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Inventory;

[Opcode(CmdIds.WeaponPromoteReq)]
public class HandlerWeaponPromoteReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = WeaponPromoteReq.Parser.ParseFrom(data);
        var result = connection.Player?.WeaponManager.PromoteWeapon(req.TargetWeaponGuid);

        if (result == null || !result.IsSuccess)
        {
            await connection.SendPacket(new BasePacket((ushort)CmdIds.WeaponPromoteRsp));
            return;
        }

        await connection.SendPacket(new PacketWeaponPromoteRsp(
            req.TargetWeaponGuid, result.OldPromoteLevel, result.NewPromoteLevel));
    }
}