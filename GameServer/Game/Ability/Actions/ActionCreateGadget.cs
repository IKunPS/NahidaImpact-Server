using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("CreateGadget")]
public class ActionCreateGadget : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: Parse AbilityActionCreateGadget proto when available
        // For now, create gadget at owner position
        var owner = ability.Owner;
        if (owner?.Scene == null) return Task.FromResult(false);

        var pos = owner.GetPosition().Clone();
        var rot = owner.GetRotation().Clone();

        // TODO: Create EntityGadget with proper constructor and config
        // owner.Scene.AddEntity(entityCreated);

        return Task.FromResult(true);
    }
}
