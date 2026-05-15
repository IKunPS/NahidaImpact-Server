using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("SetGlobalValueToOverrideMap")]
public class ActionSetGlobalValueToOverrideMap : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.SrcKey) || string.IsNullOrEmpty(action.DstKey))
            return Task.FromResult(false);

        if (target.GlobalAbilityValues.TryGetValue(action.SrcKey, out var value))
        {
            if ("DummyThrowSpeed".Equals(action.AbilityFormula))
            {
                // TODO: Apply DummyThrowSpeed formula
            }

            ability.AbilitySpecials[action.DstKey] = value;
        }

        return Task.FromResult(true);
    }
}
