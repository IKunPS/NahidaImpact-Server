using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketTakeoffEquipRsp : BasePacket
{
    public PacketTakeoffEquipRsp(ulong avatarGuid, uint slot) : base(CmdIds.TakeoffEquipRsp)
    {
        SetData(new TakeoffEquipRsp
        {
            AvatarGuid = avatarGuid,
            Slot = slot,
            Retcode = 0
        });
    }
}
