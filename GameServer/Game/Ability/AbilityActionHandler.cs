using NahidaImpact.Data.Ability;
using Google.Protobuf;
using System.Threading.Tasks;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability;

public abstract class AbilityActionHandler
{
    public AbilityManager? AbilityManager { get; set; }

    public abstract Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target);
    
    protected static BaseEntity? GetTarget(Ability ability, BaseEntity entity, string target)
    {
        return target switch
        {
            "Self" => entity,
            "Team" => ability.PlayerOwner?.TeamManager?.Entity,
            "OriginOwner" => ability.PlayerOwner?.TeamManager?.GetCurrentAvatarEntity(),
            "Owner" => ability.Owner,
            "Applier" => entity,
            "CurLocalAvatar" => ability.PlayerOwner?.TeamManager?.GetCurrentAvatarEntity(),
            "CasterOriginOwner" => null, // TODO: Figure out
            _ => null
        };
    }
}

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AbilityActionAttribute : System.Attribute
{
    public string ActionType { get; }

    public AbilityActionAttribute(string actionType)
    {
        ActionType = actionType;
    }
}
