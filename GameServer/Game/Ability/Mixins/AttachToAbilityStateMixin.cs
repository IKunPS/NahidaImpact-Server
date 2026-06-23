using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Mixins;

[AbilityMixin(AbilityMixinData.MixinType.AttachToAbilityStateMixin)]
public class AttachToAbilityStateMixin : AbilityMixinHandler
{
    // hk4e AttachToAbilityStateMixin — attaches modifiers when matching ability state is active
    public override Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target)
    {
        if (mixinData.StateIDs == null || mixinData.StateIDs.Count == 0)
            return Task.FromResult(false);

        var modifierNames = mixinData.ModifierNames;
        if (modifierNames.Count == 0) return Task.FromResult(false);

        // Check if any stateID matches currently active modifiers on the entity
        foreach (var stateId in mixinData.StateIDs)
        {
            var hasState = target.InstancedModifiers.Values
                .Any(m => m.ModifierData.State.ToString() == stateId);
            if (!hasState) continue;

            foreach (var name in modifierNames)
            {
                if (!ability.Data.Modifiers.TryGetValue(name, out var modData)) continue;
                var modCtrl = new AbilityModifierController(ability, ability.Data, modData)
                {
                    OwnerEntity = target,
                    ApplyEntityId = target.Id
                };
                ability.Modifiers[name] = modCtrl;
            }
        }

        return Task.FromResult(true);
    }
}
