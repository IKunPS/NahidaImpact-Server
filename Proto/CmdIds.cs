namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;

    // Login
    public const int GetPlayerTokenReq = 28757;
    public const int GetPlayerTokenRsp = 20813;
    public const int PlayerLoginReq = 8095;
    public const int PlayerLoginRsp = 1725;
    public const int PlayerDataNotify = 7426;
    public const int DoSetPlayerBornDataNotify = 27996;
    public const int SetPlayerBornDataReq = 29815;
    public const int SetPlayerBornDataRsp = 26559;      // not deobfuscated in 6.6

    // Enter scene
    public const int PlayerEnterSceneNotify = 684;
    public const int EnterSceneReadyReq = 25212;
    public const int EnterSceneReadyRsp = 3297;
    public const int EnterScenePeerNotify = 8028;
    public const int SceneInitFinishReq = 1892;
    public const int SceneInitFinishRsp = 21746;
    public const int PlayerEnterSceneInfoNotify = 29025;
    public const int EnterSceneDoneReq = 28765;
    public const int EnterSceneDoneRsp = 9324;
    public const int PostEnterSceneReq = 6528;
    public const int PostEnterSceneRsp = 9161;

    // Scene entity
    public const int SceneEntityAppearNotify = 1890;
    public const int SceneEntityDisappearNotify = 748;
    public const int SceneEntityMoveNotify = 7802;
    public const int SceneTimeNotify = 24341;
    public const int SceneTeamUpdateNotify = 1273;

    // Avatar
    public const int AvatarDataNotify = 21044;
    public const int AvatarAddNotify = 29741;
    public const int AvatarDelNotify = 1;
    public const int AvatarEquipChangeNotify = 23985;
    public const int AvatarFightPropNotify = 29409;
    public const int AvatarLifeStateChangeNotify = 1076;
    public const int AvatarPropNotify = 28659;
    public const int AvatarSkillChangeNotify = 28991;
    public const int AvatarSkillDepotChangeNotify = 26620;
    public const int AvatarSkillInfoNotify = 7250;
    public const int AvatarSkillMaxChargeCountNotify = 5027;
    public const int AvatarUnlockTalentNotify = 1287;
    public const int ProudSkillChangeNotify = 23487;
    public const int ProudSkillExtraLevelNotify = 7060;

    // Avatar — level / promote / skill / talent
    public const int AvatarUpgradeReq = 9051;
    public const int AvatarUpgradeRsp = 20800;
    public const int AvatarPromoteReq = 6091;
    public const int AvatarPromoteRsp = 28930;
    public const int AvatarSkillUpgradeReq = 26582;
    public const int AvatarSkillUpgradeRsp = 9601;
    public const int UnlockAvatarTalentReq = 7839;
    public const int UnlockAvatarTalentRsp = 29364;

    // Avatar — change / death
    public const int ChangeAvatarReq = 3754;
    public const int ChangeAvatarRsp = 9923;
    public const int AvatarDieAnimationEndReq = 8224;
    public const int AvatarDieAnimationEndRsp = 29421;
    public const int WorldPlayerDieNotify = 8792;
    public const int WorldPlayerInfoNotify = 26668;

    // Flycloak
    public const int AvatarWearFlycloakReq = 2845;
    public const int AvatarWearFlycloakRsp = 575;
    public const int AvatarFlycloakChangeNotify = 7359;
    public const int AvatarGainFlycloakNotify = 4708;

    // Costume
    public const int AvatarChangeCostumeReq = 23735;
    public const int AvatarChangeCostumeRsp = 7276;
    public const int AvatarChangeCostumeNotify = 222;
    public const int AvatarGainCostumeNotify = 1505;
    
    // NameCard
    public const int UnlockNameCardNotify = 21597;
    public const int GetAllUnlockNameCardReq = 29849;
    public const int GetAllUnlockNameCardRsp = 24599;
    
    public const int ServerGlobalValueChangeNotify = 24564;

    // Team
    public const int AvatarTeamUpdateNotify = 1256;
    public const int AvatarTeamAllDataNotify = 514;
    public const int ChooseCurAvatarTeamReq = 7913;
    public const int ChooseCurAvatarTeamRsp = 23619;
    public const int SetUpAvatarTeamReq = 6316;
    public const int SetUpAvatarTeamRsp = 26484;
    public const int ChangeTeamNameReq = 20869;
    public const int ChangeTeamNameRsp = 1116;
    public const int ChangeMpTeamAvatarReq = 5754;
    public const int ChangeMpTeamAvatarRsp = 27004;
    public const int AddBackupAvatarTeamReq = 9989;
    public const int AddBackupAvatarTeamRsp = 5817;
    public const int DelBackupAvatarTeamReq = 3832;
    public const int DelBackupAvatarTeamRsp = 22160;
    public const int DelTeamEntityNotify = 1;
    public const int SyncTeamEntityNotify = 24216;
    public const int SyncScenePlayTeamEntityNotify = 22166;

    // Ability
    public const int AbilityInvocationsNotify = 20808;
    public const int AbilityChangeNotify = 20280;
    public const int ClientAbilityChangeNotify = 7772;
    public const int ClientAbilityInitFinishNotify = 1623;
    public const int CombatInvocationsNotify = 26986;

    // Entity
    public const int EntityFightPropChangeReasonNotify = 4914;
    public const int EntityAiSyncNotify = 220;
    public const int EntityFightPropUpdateNotify = 24281;

    // Inventory
    public const int StoreItemChangeNotify = 20559;
    public const int StoreItemDelNotify = 3814;
    public const int PlayerStoreNotify = 24051;
    public const int ItemAddHintNotify = 24603;
    public const int TakeoffEquipReq = 26585;
    public const int TakeoffEquipRsp = 24019;
    public const int SetEquipLockStateReq = 38;
    public const int SetEquipLockStateRsp = 298;
    public const int UseItemReq = 4733;
    public const int UseItemRsp = 22767;
    public const int DestroyMaterialReq = 2800;
    public const int CalcWeaponUpgradeReturnItemsReq = 3748;
    public const int CalcWeaponUpgradeReturnItemsRsp = 542;

    // Weapon
    public const int WeaponUpgradeReq = 8208;
    public const int WeaponUpgradeRsp = 26318;
    public const int WeaponPromoteReq = 28926;
    public const int WeaponPromoteRsp = 29984;
    public const int WeaponAwakenReq = 6051;
    public const int WeaponAwakenRsp = 1084;
    public const int WearEquipReq = 25796;
    public const int WearEquipRsp = 29365;

    // Reliquary
    public const int ReliquaryUpgradeReq = 22964;
    public const int ReliquaryUpgradeRsp = 22562;
    public const int ReliquaryPromoteRsp = 6019;
    public const int ReliquaryDecomposeReq = 2545;
    public const int ReliquaryDecomposeRsp = 4930;

    // Gadget
    public const int GadgetInteractReq = 20321;
    public const int GadgetInteractRsp = 894;

    // Map / Scene points
    public const int GetScenePointReq = 25725;
    public const int GetScenePointRsp = 7515;
    public const int ScenePointUnlockNotify = 546;
    public const int SceneTransToPointReq = 7620;
    public const int SceneTransToPointRsp = 29357;
    public const int UnlockTransPointReq = 29132;
    public const int UnlockTransPointRsp = 25300;
    public const int MarkMapReq = 29102;
    public const int MarkMapRsp = 23261;
    public const int GetSceneAreaReq = 26382;
    public const int GetSceneAreaRsp = 26477;
    public const int SceneAreaUnlockNotify = 20067;
    public const int GetMapAreaReq = 6396;
    public const int GetMapAreaRsp = 28637;
    public const int MapAreaChangeNotify = 4729;
    public const int EnterWorldAreaReq = 29521;
    public const int EnterWorldAreaRsp = 24791;
    public const int PersonalSceneJumpReq = 22921;
    public const int PersonalSceneJumpRsp = 22414;
    public const int PlayerEnterMapLayerNotify = 22921;
    public const int PlayerEnterChildMapLayerNotify = 1;
    public const int PlayerWorldSceneInfoListNotify = 25510;

    // Friend / Chat
    public const int GetPlayerFriendListReq = 890;
    public const int GetPlayerFriendListRsp = 27556;
    public const int PrivateChatReq = 27651;
    public const int PrivateChatNotify = 5051;
    public const int PlayerChatReq = 23586;
    public const int PlayerChatRsp = 26416;
    public const int PlayerChatNotify = 527;
    public const int PullPrivateChatReq = 25719;
    public const int PullPrivateChatRsp = 25944;
    public const int PullRecentChatReq = 2145;
    public const int PullRecentChatRsp = 1943;

    // Profile
    public const int SetNameCardReq = 22222;
    public const int SetNameCardRsp = 26152;
    public const int SetPlayerSignatureReq = 9039;
    public const int SetPlayerSignatureRsp = 1359;
    public const int SetPlayerHeadImageReq = 27689;
    public const int SetPlayerHeadImageRsp = 781;
    public const int UpdatePlayerShowAvatarListReq = 4302;
    public const int UpdatePlayerShowAvatarListRsp = 8319;
    public const int UpdatePlayerShowNameCardListReq = 2551;
    public const int UpdatePlayerShowNameCardListRsp = 24421;
    public const int GetProfilePictureDataReq = 8977;
    public const int GetProfilePictureDataRsp = 24009;
    public const int GetPlayerSocialDetailReq = 25951;
    public const int GetPlayerSocialDetailRsp = 20887;

    // System
    public const int PingReq = 2151;
    public const int PingRsp = 26395;
    public const int ServerTimeNotify = 1622;
    public const int OpenStateUpdateNotify = 1796;
    public const int OpenStateChangeNotify = 709;
}