using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("SetGlobalValue")]
public class ActionSetGlobalValue : AbilityActionHandler
{
    // hk4e SetGlobalValueToOverrideMapImpl pattern — sets a float key on entity's global value map
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var key = action.Key;
        var value = action.Ratio;

        target.GlobalAbilityValues[key] = value;
        target.OnAbilityValueUpdate();

        return Task.FromResult(true);
    }
}
