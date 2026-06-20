using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketWeaponPromoteRsp : BasePacket
{
    public PacketWeaponPromoteRsp(ulong targetGuid, int oldPromoteLevel, int newPromoteLevel) : base(CmdIds.WeaponPromoteRsp)
    {
        SetData(new WeaponPromoteRsp
        {
            TargetWeaponGuid = targetGuid,
            OldPromoteLevel = (uint)oldPromoteLevel,
            CurPromoteLevel = (uint)newPromoteLevel,
            Retcode = 0
        });
    }
}