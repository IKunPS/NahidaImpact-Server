using NahidaImpact.Database.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketStoreItemDelNotify : BasePacket
{
    public PacketStoreItemDelNotify(ItemData item) : base(CmdIds.StoreItemDelNotify)
    {
        var proto = new StoreItemDelNotify
        {
            StoreType = StoreType.StorePack
        };
        proto.GuidList.Add(item.Guid);
        SetData(proto);
    }
}
