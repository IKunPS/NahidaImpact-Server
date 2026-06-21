using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

public class PacketClientAbilityInitFinishNotify : BasePacket
{
    public PacketClientAbilityInitFinishNotify(List<AbilityInvokeEntry> entries) : base((ushort)CmdIds.ClientAbilityInitFinishNotify)
    {
        uint entityId = 0;
        if (entries.Count > 0)
        {
            entityId = entries[0].EntityId;
        }

        var proto = new ClientAbilityInitFinishNotify
        {
            EntityId = entityId,
            Invokes = { entries }
        };
        SetData(proto);
    }
}
