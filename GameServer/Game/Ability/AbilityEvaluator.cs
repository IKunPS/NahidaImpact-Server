using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Collections.Generic;

namespace NahidaImpact.GameServer.Game.Ability;

public static class AbilityEvaluator
{
    public static bool CanAbilityBeUsed(Ability ability, BaseEntity caster)
    {
        // TODO: Implement cooldown checks, resource checks, condition checks
        return true;
    }
    
    public static bool CheckPredicates(List<object> predicates, BaseEntity caster, BaseEntity target)
    {
        // TODO: Implement predicate evaluation
        return true;
    }
    
    public static float CalculateDynamicValue(AbilityModifierAction action, BaseEntity caster,BaseEntity target)
    {
        // TODO: Implement dynamic value calculation based on action parameters
        return action.Amount;
    }
}