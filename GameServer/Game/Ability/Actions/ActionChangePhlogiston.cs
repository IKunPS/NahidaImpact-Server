using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ChangePhlogiston")]
public class ActionChangePhlogiston : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var owner = ability.Owner;
        if (owner == null) return Task.FromResult(false);

        float changeValue = action.Amount;

        // TODO: Handle Add/Lose determineType
        // TODO: Cap player at 100, vehicle at 50
        // TODO: Update phlogiston on player/vehicle

        return Task.FromResult(true);
    }
}
