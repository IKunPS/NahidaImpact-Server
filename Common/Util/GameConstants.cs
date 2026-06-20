namespace NahidaImpact.Util;

public static class GameConstants
{
    public const string GAME_VERSION = "6.6.0";
    public const int MAX_STAMINA = 300;
    public const int STAMINA_RECOVERY_TIME = 360; // 6 minutes
    public const int STAMINA_RESERVE_RECOVERY_TIME = 1080; // 18 minutes
    public const int INVENTORY_MAX_EQUIPMENT = 1000;
    public const int MAX_LINEUP_COUNT = 9;
    public const int MAX_AVATARS_IN_TEAM = 4;
    public const int DEFAULT_TEAMS = 4;
    public const int MAX_TEAMS = 10;
    public const int SERVER_CONSOLE_UID = 511694503;
    
    public const int MAIN_CHARACTER_MALE = 10000005;
    public const int MAIN_CHARACTER_FEMALE = 10000007;

    public const int DEFAULT_FLYCLOAK_ID = 340005;
    public const int DEFAULT_WEAPON_LEVEL = 1;
    public const int DEFAULT_WEAPON_PROMOTE = 0;
    public const int DEFAULT_WEAPON_REFINEMENT = 0;

    // Player defaults
    public const int START_SCENE_ID = 3; // FU**
    public const int START_REGION_ID = 1;
    public const int DEFAULT_PLAYER_LEVEL = 1;
    public const int DEFAULT_WORLD_LEVEL = 0;
    public const int DEFAULT_NAME_CARD_ID = 210001;
    public const int MAX_STAMINA_DEFAULT = 24000;
    public const int DIVE_MAX_STAMINA_DEFAULT = 10000;
    public const int PLAYER_RESIN_DEFAULT = 200;
    public const int MP_SETTING_NONE = 0;
    public const int MP_SETTING_ENTER_AFTER_APPLY = 1;
    public const int START_TELEPORT_POINT = 7;
    public const int START_SCENE_AREA = 1;
    public const int MALE_SKILL_DEPOT = 504;
    public const int FEMALE_SKILL_DEPOT = 704;

    public const float START_POS_X = 2747f;
    public const float START_POS_Y = 194f;
    public const float START_POS_Z = -1719f;
    public const float START_ROT_X = 0f;
    public const float START_ROT_Y = 307f;
    public const float START_ROT_Z = 0f;

    public static readonly string[] DEFAULT_ABILITY_STRINGS =
    {
        "Avatar_DefaultAbility_VisionReplaceDieInvincible",
        "Avatar_DefaultAbility_AvartarInShaderChange",
        "Avatar_DefaultAbility_ManualClearSurfaceTypeInSpecificState",
        "Avatar_SprintBS_Invincible",
        "Avatar_Freeze_Duration_Reducer",
        "Avatar_Attack_ReviveEnergy",
        "Avatar_Component_Initializer",
        "Avatar_FallAnthem_Achievement_Listener",
        "GrapplingHookSkill_Ability",
        "Avatar_PlayerBoy_DiveStamina_Reduction",
        "Ability_Avatar_Dive_SealEcho",
        "Absorb_SealEcho_Bullet_01",
        "Absorb_SealEcho_Bullet_02",
        "Ability_Avatar_Dive_CrabShield",
        "ActivityAbility_Absorb_Shoot",
    };

    public static readonly uint[] DEFAULT_ABILITY_HASHES =
        DEFAULT_ABILITY_STRINGS.Select(Utils.AbilityHash).ToArray();

    public static readonly string[] DEFAULT_TEAM_ABILITY_STRINGS =
    {
        "TeamAbility_Reset_Crystal_Mark",
        "Team_TeamChargeMark",
        "TeamAbility_Reset_MoonOvergrow",
        "TeamAbility_MoonPhase",
        "Ability_Avatar_Dive_Team",
    };
    
    public static uint DEFAULT_ABILITY_NAME = Utils.AbilityHash("Default");
}