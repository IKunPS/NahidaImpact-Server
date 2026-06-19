using NahidaImpact.Data.Ability;
using Google.Protobuf;
using System.Threading.Tasks;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability;

public abstract class AbilityMixinHandler
{
    public abstract Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target);
    
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
            "CasterOriginOwner" => null,
            _ => null
        };
    }
}

// AbilityMixinAttribute moved to AbilityMixinAttribute.cs