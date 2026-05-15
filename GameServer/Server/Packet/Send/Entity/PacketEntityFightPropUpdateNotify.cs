using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Entity;

public class PacketEntityFightPropUpdateNotify : BasePacket
{
    public PacketEntityFightPropUpdateNotify(BaseEntity entity, uint propType) : base((ushort)CmdIds.EntityFightPropUpdateNotify)
    {
        var proto = new EntityFightPropUpdateNotify
        {
            EntityId = entity.Id
        };
        proto.FightPropMap[propType] = entity.GetFightProperty(propType);
        SetData(proto);
    }

    public PacketEntityFightPropUpdateNotify(BaseEntity entity, IEnumerable<uint> propTypes) : base((ushort)CmdIds.EntityFightPropUpdateNotify)
    {
        var proto = new EntityFightPropUpdateNotify
        {
            EntityId = entity.Id
        };
        foreach (var propType in propTypes)
        {
            proto.FightPropMap[propType] = entity.GetFightProperty(propType);
        }
        SetData(proto);
    }
}
