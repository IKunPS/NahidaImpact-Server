namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;
    
    public const int GetPlayerTokenReq = 29446;
    public const int GetPlayerTokenRsp = 1519;

    public const int PingReq = 20405;
    public const int PingRsp = 8367;

    // Step 1
    public const int PlayerLoginReq = 5911;
    public const int PlayerDataNotify = 29964;
    public const int AvatarDataNotify = 20773;
    public const int PlayerEnterSceneNotify = 20140;
    public const int PlayerLoginRsp = 24430;

    // Step 2
    public const int EnterSceneReadyReq = 21908;
    public const int EnterScenePeerNotify = 24176;          
    public const int EnterSceneReadyRsp = 27183;

    // Step 3
    public const int SceneInitFinishReq = 20180;
    public const int PlayerEnterSceneInfoNotify = 354;
    public const int SceneTeamUpdateNotify = 22857;
    public const int SceneInitFinishRsp = 26465;

    // Step 4
    public const int EnterSceneDoneReq = 20081;
    public const int SceneEntityAppearNotify = 6288;
    public const int SceneEntityDisappearNotify = 24203;
    public const int EnterSceneDoneRsp = 1901;

    // Step 5
    public const int PostEnterSceneReq = 20074;
    public const int PostEnterSceneRsp = 536;

    public const int OpenStateUpdateNotify = 29587;
    public const int OpenStateChangeNotify = 7632;
    
    public const int DoSetPlayerBornDataNotify = 2616;
    public const int SetPlayerBornDataReq = 1;
    public const int SetPlayerBornDataRsp = 26559;
    
    
    // Ability
    public const int AbilityInvocationsNotify = 8630;
    public const int AbilityChangeNotify = 3318;
    public const int ClientAbilityChangeNotify = 43;
    public const int ClientAbilityInitFinishNotify = 21742;
    public const int SceneEntityMoveNotify = 1;
    public const int CombatInvocationsNotify = 4056;
    public const int EvtBulletMoveNotify = 1;
    
    //Entity
    public const int EntityFightPropChangeReasonNotify = 29948;
    public const int EntityAiSyncNotify = 21997;
    public const int EntityFightPropNotify = 4904; //??
    public const int EntityFightPropUpdateNotify = 8914; //??
    
    // Time
    public const int SceneTimeNotify = 1415;
    public const int ServerTimeNotify = 27941;

    // Map & Scene Points
    public const int GetScenePointReq = 27040;
    public const int GetScenePointRsp = 2025;
    public const int ScenePointUnlockNotify = 3774;
    public const int SceneTransToPointReq = 1;
    public const int SceneTransToPointRsp = 1564;
    public const int UnlockTransPointReq = 2582;
    public const int UnlockTransPointRsp = 1;
    public const int MarkMapReq = 1331;
    public const int MarkMapRsp = 1;

    // Map Area & Scene Area
    public const int GetSceneAreaReq = 24225;
    public const int GetSceneAreaRsp = 470;
    public const int SceneAreaUnlockNotify = 1;
    public const int GetMapAreaReq = 1;
    public const int GetMapAreaRsp = 9541;
    public const int MapAreaChangeNotify = 3148;
    public const int EnterWorldAreaReq = 9249;
    public const int EnterWorldAreaRsp = 20876;
    public const int SceneAreaWeatherNotify = 29909;

    // TransPoint Region
    public const int EnterTransPointRegionNotify = 1;
    public const int ExitTransPointRegionNotify = 1;

    // World Scene Info
    public const int PlayerWorldSceneInfoListNotify = 20396;

    // Personal Scene Jump
    public const int PersonalSceneJumpReq = 1;
    public const int PersonalSceneJumpRsp = 21565;

    // Map Layer
    public const int PlayerEnterMapLayerNotify = 1;
    public const int PlayerEnterChildMapLayerNotify = 1;
    
    // Friend
    public const int GetPlayerFriendListReq = 7479;
    public const int GetPlayerFriendListRsp = 27216;

    // Chat
    public const int PrivateChatReq = 26399;
    public const int PrivateChatNotify = 29577;
    public const int PlayerChatReq = 24206;
    public const int PlayerChatRsp = 1;
    public const int PlayerChatNotify = 20871;
    public const int PullPrivateChatReq = 4696;
    public const int PullPrivateChatRsp = 21477;
    public const int PullRecentChatReq = 1148;
    public const int PullRecentChatRsp = 7996;

    // Team Management
    public const int ChooseCurAvatarTeamReq = 26963;
    public const int ChooseCurAvatarTeamRsp = 1;
    public const int AddBackupAvatarTeamReq = 1;
    public const int AddBackupAvatarTeamRsp = 1;
    public const int ChangeMpTeamAvatarReq = 2419;
    public const int ChangeMpTeamAvatarRsp = 25312;
    public const int DelBackupAvatarTeamReq = 1;
    public const int DelBackupAvatarTeamRsp = 1;
    public const int ChangeTeamNameReq =1006;
    public const int ChangeTeamNameRsp = 3928;
    public const int SetUpAvatarTeamReq = 29624;
    public const int SetUpAvatarTeamRsp = 6724;
    public const int AvatarTeamUpdateNotify = 29089;
    public const int AvatarTeamAllDataNotify = 6988;
    public const int DelTeamEntityNotify = 1;
    public const int SyncTeamEntityNotify = 4567;
    public const int SyncScenePlayTeamEntityNotify = 21451;
    public const int TowerTeamSelectReq = 1;
    public const int TowerTeamSelectRsp = 1;

    // Avatar Change/Death
    public const int ChangeAvatarReq = 6317;
    public const int ChangeAvatarRsp = 23839;
    public const int AvatarDieAnimationEndReq = 1;
    public const int AvatarDieAnimationEndRsp = 1;
    public const int WorldPlayerDieNotify = 1;
    public const int WorldPlayerReviveRsp = 1;
    public const int AvatarLifeStateChangeNotify = 21104;
    public const int ServerGlobalValueChangeNotify = 28243;
    public const int AvatarAddNotify = 1;
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