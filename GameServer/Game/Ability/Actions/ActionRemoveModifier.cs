using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("RemoveModifier")]
public class ActionRemoveModifier : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);

        if (!ability.Modifiers.TryGetValue(action.ModifierName, out var modifier))
            return Task.FromResult(false);

        // hk4e: run onRemoved lifecycle before removal
        if (modifier.ModifierData.OnRemoved != null)
            foreach (var a in modifier.ModifierData.OnRemoved)
                AbilityManager?.ExecuteAction(ability, a, abilityData, target);

        ability.Modifiers.Remove(action.ModifierName);
        return Task.FromResult(true);
    }
}
