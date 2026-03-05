using NahidaImpact.Data.Ability;

namespace NahidaImpact.GameServer.Game.Ability;

public class AbilityModifierController
{
    public Ability Ability { get; }
    public AbilityData AbilityData { get; }
    public AbilityModifier ModifierData { get; }
    
    public AbilityModifierController(Ability ability, AbilityData abilityData, AbilityModifier modifierData)
    {
        Ability = ability;
        AbilityData = abilityData;
        ModifierData = modifierData;
    }
}