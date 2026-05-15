using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.State;

public class PacketServerGlobalValueChangeNotify : BasePacket
{
    public PacketServerGlobalValueChangeNotify(uint entityId, string key, float value) : base(CmdIds.ServerGlobalValueChangeNotify)
    {
        var proto = new ServerGlobalValueChangeNotify
        {
            EntityId = entityId,
            Value = value,
            KeyHash = Utils.AbilityHash(key)
        };

        SetData(proto);
    }
}
