namespace NahidaImpact.GameServer.Game.Ability;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AbilityActionAttribute(string actionType) : Attribute
{
    public string ActionType { get; } = actionType;
}