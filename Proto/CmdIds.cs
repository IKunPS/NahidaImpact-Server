namespace NahidaImpact.Proto;

public class CmdIds
{
    public const int None = 0;
    
    public const int GetPlayerTokenReq = 3184;
    public const int GetPlayerTokenRsp = 7735;

    public const int PingReq = 25884;
    public const int PingRsp = 26764;

    // Step 1
    public const int PlayerLoginReq = 9859;
    public const int PlayerDataNotify = 23430;
    public const int AvatarDataNotify = 3299;
    public const int OpenStateUpdateNotify = 28983;
    public const int PlayerEnterSceneNotify = 28874;
    public const int PlayerLoginRsp = 7986;

    // Step 2
    public const int EnterSceneReadyReq = 3656;
    public const int EnterScenePeerNotify = 835;
    public const int EnterSceneReadyRsp = 21937;

    // Step 3
    public const int SceneInitFinishReq = 23373;
    public const int PlayerEnterSceneInfoNotify = 25690;
    public const int SceneTeamUpdateNotify = 27545;
    public const int SceneInitFinishRsp = 22150;

    // Step 4
    public const int EnterSceneDoneReq = 538;
    public const int SceneEntityAppearNotify = 27052;
    public const int EnterSceneDoneRsp = 21341;

    // Step 5
    public const int PostEnterSceneReq = 9881;
    public const int PostEnterSceneRsp = 23123;

    public const int SetUpAvatarTeamReq = 6770;
    public const int SetUpAvatarTeamRsp = 22613;
    public const int AvatarTeamUpdateNotify = 24736;
    public const int DoSetPlayerBornDataNotify = 2616;
    public const int SetPlayerBornDataReq = 1;
    public const int GetPlayerFriendListReq = 24197;
    public const int GetPlayerFriendListRsp = 21879;
    public const int AbilityInvocationsNotify = 1983;
    public const int AbilityChangeNotify = 4162;
    public const int AvatarFightPropNotify = 3944;
}