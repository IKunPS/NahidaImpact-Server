using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("TriggerAbility")]
public class ActionTriggerAbility : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.AbilityName)) return Task.FromResult(false);

        var player = ability.PlayerOwner;
        if (player == null) return Task.FromResult(false);

        ability.Manager?.AddAbilityToEntity(target, action.AbilityName);

        return Task.FromResult(true);
    }
}
