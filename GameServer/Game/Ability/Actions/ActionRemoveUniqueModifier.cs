using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("RemoveUniqueModifier")]
public class ActionRemoveUniqueModifier : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        if (string.IsNullOrEmpty(action.ModifierName)) return Task.FromResult(false);

        ability.Modifiers.Remove(action.ModifierName);

        return Task.FromResult(true);
    }
}
