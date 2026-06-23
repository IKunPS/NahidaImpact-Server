using Google.Protobuf;
using NahidaImpact.Data;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("GenerateElemBall")]
public class ActionGenerateElemBall : AbilityActionHandler
{
    // hk4e GenerateElemBallImpl — spawns energy particles based on dropType
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (target.Scene == null) return Task.FromResult(false);

        var dropType = action.DropType ?? "LevelControl";
        var amount = action.Amount > 0 ? action.Amount : action.BaseEnergy;
        if (amount <= 0) return Task.FromResult(true);

        // Elemental particles drop at entity position — spawn is handled client-side
        // Server-side: track energy values for validation
        return Task.FromResult(true);
    }
}
