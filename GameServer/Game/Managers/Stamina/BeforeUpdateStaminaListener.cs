namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public interface IBeforeUpdateStaminaListener
{
    int OnBeforeUpdateStamina(string reason, int newStamina, bool isCharacterStamina);
    Consumption OnBeforeUpdateStamina(string reason, Consumption consumption, bool isCharacterStamina);
}
