namespace NahidaImpact.Prop;

public static class PlayerProp
{
    #region None

    public const uint PROP_NONE = 0;

    #endregion

    #region Avatar Stats (1001-1004)

    public const uint PROP_EXP = 1001;
    public const uint PROP_BREAK_LEVEL = 1002;
    public const uint PROP_SATIATION_VAL = 1003;
    public const uint PROP_SATIATION_PENALTY_TIME = 1004;

    #endregion

    #region Avatar Level (4001)

    public const uint PROP_LEVEL = 4001;

    #endregion

    #region Player Basic (10001-10029)

    public const uint PROP_LAST_CHANGE_AVATAR_TIME = 10001;
    public const uint PROP_MAX_SPRING_VOLUME = 10002;
    public const uint PROP_CUR_SPRING_VOLUME = 10003;
    public const uint PROP_IS_SPRING_AUTO_USE = 10004;
    public const uint PROP_SPRING_AUTO_USE_PERCENT = 10005;
    public const uint PROP_IS_FLYABLE = 10006;
    public const uint PROP_IS_WEATHER_LOCKED = 10007;
    public const uint PROP_IS_GAME_TIME_LOCKED = 10008;
    public const uint PROP_IS_TRANSFERABLE = 10009;
    public const uint PROP_MAX_STAMINA = 10010;
    public const uint PROP_CUR_PERSIST_STAMINA = 10011;
    public const uint PROP_CUR_TEMPORARY_STAMINA = 10012;
    public const uint PROP_PLAYER_LEVEL = 10013;
    public const uint PROP_PLAYER_EXP = 10014;
    public const uint PROP_PLAYER_HCOIN = 10015;
    public const uint PROP_PLAYER_SCOIN = 10016;
    public const uint PROP_PLAYER_MP_SETTING_TYPE = 10017;
    public const uint PROP_IS_MP_MODE_AVAILABLE = 10018;
    public const uint PROP_PLAYER_WORLD_LEVEL = 10019;
    public const uint PROP_PLAYER_RESIN = 10020;
    public const uint PROP_PLAYER_WAIT_SUB_HCOIN = 10022;
    public const uint PROP_PLAYER_WAIT_SUB_SCOIN = 10023;
    public const uint PROP_IS_ONLY_MP_WITH_PS_PLAYER = 10024;
    public const uint PROP_PLAYER_MCOIN = 10025;
    public const uint PROP_PLAYER_WAIT_SUB_MCOIN = 10026;
    public const uint PROP_PLAYER_LEGENDARY_KEY = 10027;
    public const uint PROP_IS_HAS_FIRST_SHARE = 10028;
    public const uint PROP_PLAYER_FORGE_POINT = 10029;

    #endregion

    #region Climate (10035-10038)

    public const uint PROP_CUR_CLIMATE_METER = 10035;
    public const uint PROP_CUR_CLIMATE_TYPE = 10036;
    public const uint PROP_CUR_CLIMATE_AREA_ID = 10037;
    public const uint PROP_CUR_CLIMATE_AREA_CLIMATE_TYPE = 10038;

    #endregion

    #region World Level (10039-10040)

    public const uint PROP_PLAYER_WORLD_LEVEL_LIMIT = 10039;
    public const uint PROP_PLAYER_WORLD_LEVEL_ADJUST_CD = 10040;

    #endregion

    #region Home & Legendary (10041-10043)

    public const uint PROP_PLAYER_LEGENDARY_DAILY_TASK_NUM = 10041;
    public const uint PROP_PLAYER_HOME_COIN = 10042;
    public const uint PROP_PLAYER_WAIT_SUB_HOME_COIN = 10043;

    #endregion

    #region GCG & Online (10044-10047)

    public const uint PROP_IS_AUTO_UNLOCK_SPECIFIC_EQUIP = 10044;
    public const uint PROP_PLAYER_GCG_COIN = 10045;
    public const uint PROP_PLAYER_WAIT_SUB_GCG_COIN = 10046;
    public const uint PROP_PLAYER_ONLINE_TIME = 10047;

    #endregion

    #region Dive (10048-10050)

    public const uint PROP_PLAYER_CAN_DIVE = 10048;
    public const uint PROP_DIVE_MAX_STAMINA = 10049;
    public const uint PROP_DIVE_CUR_STAMINA = 10050;

    #endregion

    #region Phlogiston / Natlan (10052-10054)

    public const uint PROP_PHLOGISTON_ENABLE = 10052;
    public const uint PROP_PHLOGISTON_MAX_VALUE = 10053;
    public const uint PROP_CUR_PHLOGISTON = 10054;

    #endregion
}
