using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Entity;

public class PacketEntityFightPropChangeReasonNotify : BasePacket
{
    /// <summary>Basic constructor: prop, delta, reason, hp change reason.</summary>
    public PacketEntityFightPropChangeReasonNotify(
        BaseEntity entity,
        uint propType,
        float value,
        PropChangeReason reason,
        ChangeHpReason changeHpReason) : base((ushort)CmdIds.EntityFightPropChangeReasonNotify)
    {
        var proto = new EntityFightPropChangeReasonNotify
        {
            EntityId = entity.Id,
            PropType = propType,
            PropDelta = value,
            Reason = reason,
            ChangeHpReason = changeHpReason
        };

        SetData(proto);
    }

    /// <summary>Constructor with HP debts reason.</summary>
    public PacketEntityFightPropChangeReasonNotify(
        BaseEntity entity,
        uint propType,
        float value,
        PropChangeReason reason,
        ChangeHpDebtsReason changeHpDebtsReason) : base((ushort)CmdIds.EntityFightPropChangeReasonNotify)
    {
        var proto = new EntityFightPropChangeReasonNotify
        {
            EntityId = entity.Id,
            PropType = propType,
            PropDelta = value,
            Reason = reason,
            ChangeHpDebtsReason = changeHpDebtsReason
        };

        SetData(proto);
    }

    /// <summary>Constructor with energy change reason.</summary>
    public PacketEntityFightPropChangeReasonNotify(
        BaseEntity entity,
        uint propType,
        float value,
        PropChangeReason reason,
        ChangeEnergyReason energyReason) : base((ushort)CmdIds.EntityFightPropChangeReasonNotify)
    {
        var proto = new EntityFightPropChangeReasonNotify
        {
            EntityId = entity.Id,
            PropType = propType,
            PropDelta = value,
            Reason = reason,
            ChangeEnergyReason = energyReason
        };

        SetData(proto);
    }

    /// <summary>Constructor without sub-reason (props only).</summary>
    public PacketEntityFightPropChangeReasonNotify(
        BaseEntity entity,
        uint propType,
        float value,
        PropChangeReason reason) : base((ushort)CmdIds.EntityFightPropChangeReasonNotify)
    {
        var proto = new EntityFightPropChangeReasonNotify
        {
            EntityId = entity.Id,
            PropType = propType,
            PropDelta = value,
            Reason = reason
        };

        SetData(proto);
    }
}
