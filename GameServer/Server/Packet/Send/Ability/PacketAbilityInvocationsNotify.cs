using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using System.Collections.Generic;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

public class PacketAbilityInvocationsNotify : BasePacket
{
    public PacketAbilityInvocationsNotify(List<AbilityInvokeEntry> entries) : base(CmdIds.AbilityInvocationsNotify)
    {
        var notify = new AbilityInvocationsNotify();
        notify.Invokes.AddRange(entries);

        SetData(notify);
    }

    public PacketAbilityInvocationsNotify(AbilityInvokeEntry entry) : base(CmdIds.AbilityInvocationsNotify)
    {
        var notify = new AbilityInvocationsNotify();
        notify.Invokes.Add(entry);

        SetData(notify);
    }
}
