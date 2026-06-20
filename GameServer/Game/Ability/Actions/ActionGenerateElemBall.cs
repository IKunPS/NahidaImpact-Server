using Google.Protobuf;
using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("GenerateElemBall")]
public class ActionGenerateElemBall : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: Handle drop types (LevelControl, BigWorldOnly, ForceDrop)
        // TODO: Calculate energy amount from baseEnergy * ratio
        // TODO: Look up item data and spawn EntityItem energy balls

        return Task.FromResult(true);
    }
}
