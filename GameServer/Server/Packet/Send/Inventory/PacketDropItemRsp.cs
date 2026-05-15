using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketDropItemRsp : BasePacket
{
    public PacketDropItemRsp(ulong guid, StoreType storeType = StoreType.StorePack) : base(CmdIds.DropItemRsp)
    {
        SetData(new DropItemRsp
        {
            Guid = guid,
            StoreType = storeType,
            Retcode = 0
        });
    }
}
