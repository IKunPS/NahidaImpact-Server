using NahidaImpact.Data.Ability;
using Google.Protobuf;
using System.Threading.Tasks;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability;

public abstract class AbilityMixinHandler
{
    public abstract Task<bool> Execute(Ability ability, AbilityMixinData mixinData, ByteString abilityData, BaseEntity target);
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AbilityMixinAttribute : Attribute
{
    public AbilityMixinData.MixinType MixinType { get; }

    public AbilityMixinAttribute(AbilityMixinData.MixinType mixinType)
    {
        MixinType = mixinType;
    }
}