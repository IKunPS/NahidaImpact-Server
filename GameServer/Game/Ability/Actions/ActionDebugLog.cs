using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("DebugLog")]
public class ActionDebugLog : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        var logger = new Util.Logger("ActionDebugLog");
        logger.Debug($"Ability DebugLog: {action.Content}");

        return Task.FromResult(true);
    }
}
