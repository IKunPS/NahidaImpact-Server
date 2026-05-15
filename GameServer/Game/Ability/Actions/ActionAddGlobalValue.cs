using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("AddGlobalValue")]
public class ActionAddGlobalValue : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner == null) return Task.FromResult(false);

        // Collect fight properties as a flat dictionary
        var properties = new System.Collections.Generic.Dictionary<string, float>();
        foreach (var fp in owner.FightProperties)
            properties[fp.PropType.ToString()] = fp.PropValue;
        foreach (var kv in ability.AbilitySpecials)
            properties[kv.Key] = kv.Value;

        string valueKey = action.Key;
        float valueToAdd = action.Amount;

        // Clamp by min/max
        if (action.UseLimitRange)
        {
            valueToAdd = System.Math.Max(action.MinValue, System.Math.Min(action.MaxValue, valueToAdd));
        }

        if (target.GlobalAbilityValues.TryGetValue(valueKey, out var currentValue))
            target.GlobalAbilityValues[valueKey] = currentValue + valueToAdd;
        else
            target.GlobalAbilityValues[valueKey] = valueToAdd;

        target.OnAbilityValueUpdate();

        return Task.FromResult(true);
    }
}
