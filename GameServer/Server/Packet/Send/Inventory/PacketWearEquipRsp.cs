using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketWearEquipRsp : BasePacket
{
    public PacketWearEquipRsp(ulong avatarGuid, ulong equipGuid) : base(CmdIds.WearEquipRsp)
    {
        SetData(new WearEquipRsp
        {
            AvatarGuid = avatarGuid,
            EquipGuid = equipGuid,
            Retcode = 0
        });
    }
}
