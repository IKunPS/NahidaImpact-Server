using Google.Protobuf;
using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Entity;
using System.Threading.Tasks;

namespace NahidaImpact.GameServer.Game.Ability.Actions;

[AbilityAction("ServerLuaCall")]
public class ActionServerLuaCall : AbilityActionHandler
{
    public override Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target)
    {
        // TODO: Handle LuaCallType (FromGroup, SpecificGroup, Gadget)
        // TODO: Call group scripts or gadget controllers

        return Task.FromResult(true);
    }
}
