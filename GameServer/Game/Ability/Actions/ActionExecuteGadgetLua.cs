using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ExecuteGadgetLua")]
public class ActionExecuteGadgetLua : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        ability.Owner?.OnClientExecuteRequest((int)action.Param1, (int)action.Param2, (int)action.Param3);

        return Task.FromResult(true);
    }
}
