using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketWeaponUpgradeRsp : BasePacket
{
    public PacketWeaponUpgradeRsp(ulong targetGuid, int oldLevel, int newLevel, List<ItemParam> returnItems) : base(CmdIds.WeaponUpgradeRsp)
    {
        var proto = new WeaponUpgradeRsp
        {
            TargetWeaponGuid = targetGuid,
            OldLevel = (uint)oldLevel,
            CurLevel = (uint)newLevel,
            Retcode = 0
        };
        proto.ItemParamList.AddRange(returnItems);
        SetData(proto);
    }
}