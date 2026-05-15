namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public interface AfterUpdateStaminaListener
{
    void OnAfterUpdateStamina(string reason, int newStamina, bool isCharacterStamina);
}
