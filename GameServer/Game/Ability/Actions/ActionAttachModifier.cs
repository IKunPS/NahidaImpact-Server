using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AttachModifier")]
public class ActionAttachModifier : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);
        if (!ability.Data.Modifiers.TryGetValue(action.ModifierName, out var modifierData))
            return Task.FromResult(false);

        var modifier = new AbilityModifierController(ability, ability.Data, modifierData);
        ability.Modifiers[action.ModifierName] = modifier;

        return Task.FromResult(true);
    }
}
