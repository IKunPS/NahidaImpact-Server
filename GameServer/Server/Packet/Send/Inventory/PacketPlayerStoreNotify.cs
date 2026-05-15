using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketPlayerStoreNotify : BasePacket
{
    public PacketPlayerStoreNotify(List<ItemData> items) : base(CmdIds.PlayerStoreNotify)
    {
        var proto = new PlayerStoreNotify
        {
            StoreType = StoreType.StorePack,
            WeightLimit = 30000
        };
        foreach (var item in items)
            proto.ItemList.Add(item.ToProto());
        SetData(proto);
    }
}
