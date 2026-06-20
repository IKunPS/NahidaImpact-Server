using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ChangePhlogiston")]
public class ActionChangePhlogiston : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner == null) return Task.FromResult(false);

        var player = owner.Owner;
        if (player == null) return Task.FromResult(false);

        float changeValue = action.Amount;

        if (action.DetermineType == "Add")
        {
            float currentPhlogiston = player.GetPhlogistonValue();
            float newValue = currentPhlogiston + changeValue;
            if (newValue > 100) newValue = 100;
            player.SetPhlogistonValue(newValue);
        }
        else if (action.DetermineType == "Lose")
        {
            float currentPhlogiston = player.GetPhlogistonValue();
            float newValue = currentPhlogiston - changeValue;
            if (newValue < 0) newValue = 0;
            player.SetPhlogistonValue(newValue);
        }

        return Task.FromResult(true);
    }
}