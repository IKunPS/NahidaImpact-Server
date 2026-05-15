using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.Prop;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ReviveElemEnergy")]
public class ActionReviveElemEnergy : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        float ratio = action.Ratio;

        // TODO: Avatar 10000097 special handling
        // TODO: Get avatar's current energy type, add energy

        return Task.FromResult(true);
    }
}
