using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketCalcWeaponUpgradeReturnItemsRsp : BasePacket
{
    public PacketCalcWeaponUpgradeReturnItemsRsp(ulong targetGuid, List<ItemParam> returnItems) : base(CmdIds.CalcWeaponUpgradeReturnItemsRsp)
    {
        var proto = new CalcWeaponUpgradeReturnItemsRsp
        {
            TargetWeaponGuid = targetGuid,
            Retcode = 0
        };
        proto.ItemParamList.AddRange(returnItems);
        SetData(proto);
    }
}