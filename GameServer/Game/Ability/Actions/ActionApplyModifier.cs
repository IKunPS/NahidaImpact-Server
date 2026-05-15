using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ApplyModifier")]
public class ActionApplyModifier : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);

        var modifierData = ability.Data.Modifiers.GetValueOrDefault(action.ModifierName);
        if (modifierData == null) return Task.FromResult(false);

        // Unique stacking check
        if (modifierData.Stacking == "Unique"
            && ability.Modifiers.Values.Any(m => m.ModifierData == modifierData))
        {
            return Task.FromResult(true);
        }

        var modifier = new AbilityModifierController(ability, ability.Data, modifierData);
        ability.Modifiers[action.ModifierName] = modifier;

        if (modifierData.OnAdded != null)
        {
            foreach (var a in modifierData.OnAdded)
                AbilityManager?.ExecuteAction(ability, a, abilityData, target);
        }
        if (modifierData.OnAttackLanded != null)
        {
            foreach (var b in modifierData.OnAttackLanded)
                AbilityManager?.ExecuteAction(ability, b, abilityData, target);
        }

        return Task.FromResult(true);
    }
}
