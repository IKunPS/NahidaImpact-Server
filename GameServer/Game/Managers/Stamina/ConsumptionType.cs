namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public enum ConsumptionType
{
    None = 0,

    // Consume
    CLIMBING = -150,
    CLIMB_START = -500,
    CLIMB_JUMP = -2500,
    DASH = -360,
    FIGHT = 0, // See StaminaManager.getFightConsumption()
    FLY = -60,
    SKIFF_DASH = -204,
    SPRINT = -1800,
    SWIM_DASH_START = -2000,
    SWIM_DASH = -204,
    SWIMMING = -80,
    TALENT_DASH = -300,
    TALENT_DASH_START = -1000,

    // Restore
    POWERED_FLY = 500,
    POWERED_SKIFF = 500,
    RUN = 500,
    SKIFF = 500,
    STANDBY = 500,
    WALK = 500,
    NOTIFY = 500
}
