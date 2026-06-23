using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Linq;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ApplyModifier")]
public class ActionApplyModifier : AbilityActionHandler
{
    // hk4e ApplyModifier — instantiates modifier on ability owner, runs onAdded/onRemoved lifecycle
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);

        if (!ability.Data.Modifiers.TryGetValue(action.ModifierName, out var modifierData))
            return Task.FromResult(false);

        // Unique stacking check
        if (modifierData.Stacking == "Unique"
            && ability.Modifiers.Values.Any(m => m.ModifierData == modifierData))
            return Task.FromResult(true);

        var modifier = new AbilityModifierController(ability, ability.Data, modifierData)
        {
            OwnerEntity = target,
            ApplyEntityId = target.Id,
            StartTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        ability.Modifiers[action.ModifierName] = modifier;

        if (modifierData.OnAdded != null)
            foreach (var a in modifierData.OnAdded)
                AbilityManager?.ExecuteAction(ability, a, abilityData, target);

        if (modifierData.OnAttackLanded != null)
            foreach (var a in modifierData.OnAttackLanded)
                AbilityManager?.ExecuteAction(ability, a, abilityData, target);

        return Task.FromResult(true);
    }
}
