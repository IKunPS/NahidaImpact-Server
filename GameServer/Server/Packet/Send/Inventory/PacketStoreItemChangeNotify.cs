using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketStoreItemChangeNotify : BasePacket
{
    public PacketStoreItemChangeNotify(ItemData item) : base(CmdIds.StoreItemChangeNotify)
    {
        var proto = new StoreItemChangeNotify
        {
            StoreType = StoreType.StorePack
        };
        proto.ItemList.Add(item.ToProto());
        SetData(proto);
    }

    public PacketStoreItemChangeNotify(List<ItemData> items) : base(CmdIds.StoreItemChangeNotify)
    {
        var proto = new StoreItemChangeNotify
        {
            StoreType = StoreType.StorePack
        };
        foreach (var item in items)
            proto.ItemList.Add(item.ToProto());
        SetData(proto);
    }
}
