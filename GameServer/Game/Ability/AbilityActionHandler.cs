using NahidaImpact.Data.Ability;
using Google.Protobuf;
using System.Threading.Tasks;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Game.Ability;

public abstract class AbilityActionHandler
{
    public abstract Task<bool> Execute(Ability ability, AbilityModifierAction action, ByteString abilityData, BaseEntity target);
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AbilityActionAttribute : Attribute
{
    public string ActionType { get; }

    public AbilityActionAttribute(string actionType)
    {
        ActionType = actionType;
    }
}