using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Collections.Generic;
using System.Linq;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

/// <summary>
/// Sends ClientAbilityInitFinishNotify with invoke entries and entity ID.
/// Mirrors Java PacketClientAbilityInitFinishNotify.
/// </summary>
public class PacketClientAbilityInitFinishNotify : BasePacket
{
    public PacketClientAbilityInitFinishNotify(List<AbilityInvokeEntry> entries) : base((ushort)CmdIds.ClientAbilityInitFinishNotify)
    {
        // TODO: Use ClientAbilityInitFinishNotify proto when generated.
        // For now, use AbilityInvocationsNotify which has the same invokes list structure.
        uint entityId = 0;
        if (entries.Count > 0)
        {
            entityId = entries[0].EntityId;
        }

        // When ClientAbilityInitFinishNotify C# proto is available, replace with:
        var proto = new ClientAbilityInitFinishNotify
        {
            EntityId = entityId,
            Invokes = { entries }
        };
        SetData(proto);
    }
}
