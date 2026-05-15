using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ClearGlobalValue")]
public class ActionClearGlobalValue : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.Key)) return Task.FromResult(false);

        target.GlobalAbilityValues.Remove(action.Key);
        target.OnAbilityValueUpdate();

        return Task.FromResult(true);
    }
}
