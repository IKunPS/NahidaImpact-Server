using NahidaImpact.Data.Ability;

namespace NahidaImpact.GameServer.Game.Ability;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AbilityMixinAttribute(AbilityMixinData.MixinType mixinType) : Attribute
{
    public AbilityMixinData.MixinType MixinType { get; } = mixinType;
}