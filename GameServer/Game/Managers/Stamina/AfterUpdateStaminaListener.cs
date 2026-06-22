namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public interface IAfterUpdateStaminaListener
{
    void OnAfterUpdateStamina(string reason, int newStamina, bool isCharacterStamina);
}
