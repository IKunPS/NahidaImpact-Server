using System.Collections.Generic;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

public class PacketCombatInvocationsNotify : BasePacket
{
    public PacketCombatInvocationsNotify(List<CombatInvokeEntry> entries) : base(CmdIds.CombatInvocationsNotify)
    {
        var notify = new CombatInvocationsNotify();
        notify.InvokeList.AddRange(entries);

        SetData(notify);
    }
}
