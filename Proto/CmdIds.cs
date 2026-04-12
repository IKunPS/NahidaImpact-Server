namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;
    
    public const int GetPlayerTokenReq = 22738;
    public const int GetPlayerTokenRsp = 4838;

    public const int PingReq = 29231;
    public const int PingRsp = 24017;

    // Step 1
    public const int PlayerLoginReq = 21034;
    public const int PlayerDataNotify = 5677;
    public const int AvatarDataNotify = 26997;
    public const int PlayerEnterSceneNotify = 9304;
    public const int PlayerLoginRsp = 21346;

    // Step 2
    public const int EnterSceneReadyReq = 20696;
    public const int EnterScenePeerNotify = 2015;          
    public const int EnterSceneReadyRsp = 9877;

    // Step 3
    public const int SceneInitFinishReq = 25507;
    public const int PlayerEnterSceneInfoNotify = 23085;
    public const int SceneTeamUpdateNotify = 21351;
    public const int SceneInitFinishRsp = 26252;

    // Step 4
    public const int EnterSceneDoneReq = 28913;
    public const int SceneEntityAppearNotify = 5582;
    public const int EnterSceneDoneRsp = 7024;

    // Step 5
    public const int PostEnterSceneReq = 2725;
    public const int PostEnterSceneRsp = 8325;

    public const int OpenStateUpdateNotify = 287;
    public const int SetUpAvatarTeamReq = 6770;
    public const int SetUpAvatarTeamRsp = 22613;
    public const int ChangeTeamNameRsp = 8309;
    public const int AvatarTeamUpdateNotify = 24736;
    public const int DoSetPlayerBornDataNotify = 2616;
    public const int SetPlayerBornDataReq = 1;
    public const int GetPlayerFriendListReq = 29038;
    public const int GetPlayerFriendListRsp = 27344;
    public const int AbilityInvocationsNotify = 1983;
    public const int AbilityChangeNotify = 4162;
    public const int AvatarFightPropNotify = 3944;
    public const int SceneEntityDisappearNotify = 24203;
    public const int PlayerChatNotify = 6569;
}