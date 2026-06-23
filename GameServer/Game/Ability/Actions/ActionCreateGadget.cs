using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("CreateGadget")]
public class ActionCreateGadget : AbilityActionHandler
{
    // hk4e CreateGadgetImpl — creates a gadget entity at the specified position from the client proto
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner?.Scene == null) return Task.FromResult(false);

        var gadgetId = action.GadgetID > 0 ? action.GadgetID : action.ConfigID;
        if (gadgetId <= 0) return Task.FromResult(false);

        Position pos;
        Position rot;
        try
        {
            var proto = AbilityActionCreateGadget.Parser.ParseFrom(abilityData);
            pos = proto.Pos != null ? new Position(proto.Pos) : owner.Position.Clone();
            rot = proto.Rot != null ? new Position(proto.Rot) : owner.Rotation.Clone();
        }
        catch
        {
            pos = owner.Position.Clone();
            rot = owner.Rotation.Clone();
        }

        var gadget = new EntityGadget(owner.Scene, gadgetId, pos, rot)
        {
            BornType = GadgetBornType.Gadget
        };
        owner.Scene.AddEntity(gadget);

        return Task.FromResult(true);
    }
}
