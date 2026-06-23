using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AttachModifier")]
public class ActionAttachModifier : AbilityActionHandler
{
    // hk4e AttachModifier — attaches a modifier to a target entity (differs from ApplyModifier which self-applies)
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);
        if (!ability.Data.Modifiers.TryGetValue(action.ModifierName, out var modifierData))
            return Task.FromResult(false);

        var modifier = new AbilityModifierController(ability, ability.Data, modifierData)
        {
            OwnerEntity = target,
            ApplyEntityId = target.Id,
            IsAttachedParentAbility = true,
            StartTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        ability.Modifiers[action.ModifierName] = modifier;

        if (modifierData.OnAdded != null)
            foreach (var a in modifierData.OnAdded)
                AbilityManager?.ExecuteAction(ability, a, abilityData, target);

        return Task.FromResult(true);
    }
}
