using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("CIKIKFMNALB")]
public class ActionCIKIKFMNALB : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: If owner is EntityGadget, call gadget.updateState(action.StateID)

        return Task.FromResult(true);
    }
}
