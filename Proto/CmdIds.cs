namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;
    
    public const int GetPlayerTokenReq = 20352;
    public const int GetPlayerTokenRsp = 5913;

    public const int PingReq = 8256;
    public const int PingRsp = 7774;

    // Step 1
    public const int PlayerLoginReq = 27093;
    public const int PlayerDataNotify = 4835;
    public const int AvatarDataNotify = 1224;
    public const int PlayerEnterSceneNotify = 28235;
    public const int PlayerLoginRsp = 25444;

    // Step 2
    public const int EnterSceneReadyReq = 25395;
    public const int EnterScenePeerNotify = 29234;
    public const int EnterSceneReadyRsp = 23190;

    // Step 3
    public const int SceneInitFinishReq = 29293;
    public const int PlayerEnterSceneInfoNotify = 2019;
    public const int SceneTeamUpdateNotify = 7975;
    public const int SceneInitFinishRsp = 28055;

    // Step 4
    public const int EnterSceneDoneReq = 9122;
    public const int SceneEntityAppearNotify = 5768;
    public const int EnterSceneDoneRsp = 22959;

    // Step 5
    public const int PostEnterSceneReq = 26806;
    public const int PostEnterSceneRsp = 28563;

    public const int OpenStateUpdateNotify = 22147;
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