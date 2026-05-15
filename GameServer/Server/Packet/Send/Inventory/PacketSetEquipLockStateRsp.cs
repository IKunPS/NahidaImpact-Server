using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketSetEquipLockStateRsp : BasePacket
{
    public PacketSetEquipLockStateRsp(ulong equipGuid, bool isLocked) : base(CmdIds.SetEquipLockStateRsp)
    {
        SetData(new SetEquipLockStateRsp
        {
            TargetEquipGuid = equipGuid,
            IsLocked = isLocked,
            Retcode = 0
        });
    }
}
