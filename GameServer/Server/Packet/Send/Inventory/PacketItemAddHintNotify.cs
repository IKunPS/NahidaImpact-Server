using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Inventory;

public class PacketItemAddHintNotify : BasePacket
{
    public PacketItemAddHintNotify(ItemData item, uint reason = 0) : base(CmdIds.ItemAddHintNotify)
    {
        var proto = new ItemAddHintNotify
        {
            Reason = reason
        };
        proto.ItemList.Add(item.ToItemHintProto());
        SetData(proto);
    }

    public PacketItemAddHintNotify(List<ItemData> items, uint reason = 0) : base(CmdIds.ItemAddHintNotify)
    {
        var proto = new ItemAddHintNotify
        {
            Reason = reason
        };
        foreach (var item in items)
            proto.ItemList.Add(item.ToItemHintProto());
        SetData(proto);
    }
}
