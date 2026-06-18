namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;
    
    public const int GetPlayerTokenReq = 28757;
    public const int GetPlayerTokenRsp = 20813;

    public const int PingReq = 2151;
    public const int PingRsp = 26395;

    // Step 1
    public const int PlayerLoginReq = 8095;
    public const int PlayerDataNotify = 7426;
    public const int AvatarDataNotify = 21044;
    public const int PlayerEnterSceneNotify = 684;
    public const int PlayerLoginRsp = 1725;

    // Step 2
    public const int EnterSceneReadyReq = 25212;
    public const int EnterScenePeerNotify = 8028;          
    public const int EnterSceneReadyRsp = 3297;

    // Step 3
    public const int SceneInitFinishReq = 1892;
    public const int PlayerEnterSceneInfoNotify = 29025;
    public const int SceneTeamUpdateNotify = 1273;
    public const int SceneInitFinishRsp = 21746;

    // Step 4
    public const int EnterSceneDoneReq = 28765;
    public const int SceneEntityAppearNotify = 1890;
    public const int SceneEntityDisappearNotify = 748;
    public const int EnterSceneDoneRsp = 9324;

    // Step 5
    public const int PostEnterSceneReq = 6528;
    public const int PostEnterSceneRsp = 9161;

    public const int OpenStateUpdateNotify = 1796;
    public const int OpenStateChangeNotify = 709;
    
    public const int DoSetPlayerBornDataNotify = 27996;
    public const int SetPlayerBornDataReq = 29815;
    public const int SetPlayerBornDataRsp = 26559;
    
    
    // Ability
    public const int AbilityInvocationsNotify = 20808;
    public const int AbilityChangeNotify = 20280;
    public const int ClientAbilityChangeNotify = 7772;
    public const int ClientAbilityInitFinishNotify = 1623;
    public const int SceneEntityMoveNotify = 1;
    public const int CombatInvocationsNotify = 26986;
    
    //Entity
    public const int EntityFightPropChangeReasonNotify = 4914;
    public const int EntityAiSyncNotify = 220;
    public const int EntityFightPropNotify = 2271;
    public const int EntityFightPropUpdateNotify = 24281;
    
    // Time
    public const int SceneTimeNotify = 24341;
    public const int ServerTimeNotify = 1622;

    // Map & Scene Points
    public const int GetScenePointReq = 25725;
    public const int GetScenePointRsp = 7515;
    public const int ScenePointUnlockNotify = 546;
    public const int SceneTransToPointReq = 7620;
    public const int SceneTransToPointRsp = 29357;
    public const int UnlockTransPointReq = 29132;
    public const int UnlockTransPointRsp = 25300;
    public const int MarkMapReq = 29102;
    public const int MarkMapRsp = 23261;

    // Map Area & Scene Area
    public const int GetSceneAreaReq = 26382;
    public const int GetSceneAreaRsp = 26477;
    public const int SceneAreaUnlockNotify = 20067;
    public const int GetMapAreaReq = 6396;
    public const int GetMapAreaRsp = 28637;
    public const int MapAreaChangeNotify = 4729;
    public const int EnterWorldAreaReq = 29521;
    public const int EnterWorldAreaRsp = 24791;
    public const int SceneAreaWeatherNotify = 29909;

    // TransPoint Region
    public const int EnterTransPointRegionNotify = 1;
    public const int ExitTransPointRegionNotify = 1;

    // World Scene Info
    public const int PlayerWorldSceneInfoListNotify = 25510;

    // Personal Scene Jump
    public const int PersonalSceneJumpReq = 1;
    public const int PersonalSceneJumpRsp = 21565;

    // Map Layer
    public const int PlayerEnterMapLayerNotify = 22921;
    public const int PlayerEnterChildMapLayerNotify = 1;
    
    // Friend
    public const int GetPlayerFriendListReq = 890;
    public const int GetPlayerFriendListRsp = 27556;

    // Chat
    public const int PrivateChatReq = 27651;
    public const int PrivateChatNotify = 5051;
    public const int PlayerChatReq = 23586;
    public const int PlayerChatRsp = 26416;
    public const int PlayerChatNotify = 527;
    public const int PullPrivateChatReq = 25719;
    public const int PullPrivateChatRsp = 25944;
    public const int PullRecentChatReq = 2145;
    public const int PullRecentChatRsp = 1943;

    // Team Management
    public const int ChooseCurAvatarTeamReq = 26963;
    public const int ChooseCurAvatarTeamRsp = 1;
    public const int AddBackupAvatarTeamReq = 1;
    public const int AddBackupAvatarTeamRsp = 1;
    public const int ChangeMpTeamAvatarReq = 2419;
    public const int ChangeMpTeamAvatarRsp = 25312;
    public const int DelBackupAvatarTeamReq = 1;
    public const int DelBackupAvatarTeamRsp = 1;
    public const int ChangeTeamNameReq = 20869;
    public const int ChangeTeamNameRsp = 1116;
    public const int SetUpAvatarTeamReq = 6316;
    public const int SetUpAvatarTeamRsp = 26484;
    public const int AvatarTeamUpdateNotify = 1256;
    public const int AvatarTeamAllDataNotify = 514;
    public const int DelTeamEntityNotify = 1;
    public const int SyncTeamEntityNotify = 24216;
    public const int SyncScenePlayTeamEntityNotify = 22166;
    public const int TowerTeamSelectReq = 1;
    public const int TowerTeamSelectRsp = 1;

    // Avatar Change/Death
    public const int ChangeAvatarReq = 3754;
    public const int ChangeAvatarRsp = 9923;
    public const int AvatarDieAnimationEndReq = 1;
    public const int AvatarDieAnimationEndRsp = 1;
    public const int WorldPlayerDieNotify = 1;
    public const int WorldPlayerReviveRsp = 1;
    public const int AvatarLifeStateChangeNotify = 21104;
    public const int ServerGlobalValueChangeNotify = 28243;
    public const int AvatarAddNotify = 29741;
    public const int AvatarDelNotify = 1;
    
    public const int AvatarFightPropNotify = 3944;

    // Inventory
    public const int StoreItemChangeNotify = 27681;
    public const int StoreItemDelNotify = 8176;
    public const int PlayerStoreNotify = 24081;
    public const int ItemAddHintNotify = 1931;
    public const int ItemCdGroupTimeNotify = 2210;
    public const int DropItemReq = 28166;
    public const int DropItemRsp = 4794;
    public const int WearEquipReq = 20553;
    public const int WearEquipRsp = 22987;
    public const int TakeoffEquipReq = 3700;
    public const int TakeoffEquipRsp = 28745;
    public const int SetEquipLockStateReq = 21501;
    public const int SetEquipLockStateRsp = 4504;
    public const int WeaponUpgradeReq = 4010;
    public const int WeaponUpgradeRsp = 2053;
    public const int WeaponPromoteReq = 23264;
    public const int WeaponPromoteRsp = 29840;
    public const int WeaponAwakenReq = 3041;
    public const int WeaponAwakenRsp = 849;
    public const int ReliquaryUpgradeReq = 6817;
    public const int ReliquaryUpgradeRsp = 28013;
    public const int ReliquaryPromoteReq = 3645;
    public const int ReliquaryPromoteRsp = 21845;
    public const int ReliquaryDecomposeReq = 26691;
    public const int ReliquaryDecomposeRsp = 8828;
    public const int DestroyMaterialReq = 5054;
    public const int DestroyMaterialRsp = 23522;
    public const int UseItemReq = 4800;
    public const int UseItemRsp = 29891;
    public const int CalcWeaponUpgradeReturnItemsRsp = 1588;
    public const int AvatarEquipChangeNotify = 1605;
    
    // Gadget
    public const int GadgetInteractReq = 143;
}