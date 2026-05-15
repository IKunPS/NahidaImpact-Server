using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Entity;

public class PacketEntityFightPropNotify : BasePacket
{
    public PacketEntityFightPropNotify(BaseEntity entity) : base((ushort)CmdIds.EntityFightPropNotify)
    {
        var proto = new EntityFightPropNotify
        {
            EntityId = entity.Id
        };
        foreach (var kv in entity.GetFightProperties())
        {
            if (kv.Key == 0) continue;
            proto.FightPropMap[(uint)kv.Key] = kv.Value;
        }
        SetData(proto);
    }
}
