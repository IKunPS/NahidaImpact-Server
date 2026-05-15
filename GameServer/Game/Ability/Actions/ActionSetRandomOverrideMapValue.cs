using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("SetRandomOverrideMapValue")]
public class ActionSetRandomOverrideMapValue : AbilityActionHandler
{
    private static readonly Random Random = new();

    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.OverrideMapKey)) return Task.FromResult(false);

        // TODO: Parse random value from proto
        float randomValue = (float)Random.NextDouble() * (action.ValueRangeMax - action.ValueRangeMin) + action.ValueRangeMin;

        ability.AbilitySpecials[action.OverrideMapKey] = randomValue;

        return Task.FromResult(true);
    }
}
