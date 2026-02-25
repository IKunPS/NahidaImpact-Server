using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Friends;

public class Friendship
{
    public uint FriendUid { get; set; }
    public PlayerProfile FriendProfile { get; set; } = null!;
    public bool IsMpModeAvailable { get; set; }

    public Friendship(uint friendUid, PlayerProfile profile)
    {
        FriendUid = friendUid;
        FriendProfile = profile;
    }

    public bool IsOnline()
    {
        var player = PlayerInstance.GetPlayerInstanceByUid(FriendUid);
        return player?.Connection?.IsOnline == true;
    }

    public FriendBrief ToProto()
    {
        var isOnline = IsOnline();
        var proto = new FriendBrief
        {
            Uid = FriendUid,
            Nickname = FriendProfile.Nickname ?? "",
            Level = FriendProfile.Level,
            ProfilePicture = FriendProfile.HeadImage,
            WorldLevel = FriendProfile.WorldLevel,
            Signature = FriendProfile.Signature ?? "",
            OnlineState = isOnline ? FriendOnlineState.FriendOnline : FriendOnlineState.FreiendDisconnect,
            LastActiveTime = (uint)FriendProfile.LastActiveTime,
            NameCardId = FriendProfile.NameCardId,
            Param = FriendProfile.DaysSinceLastLogin,
            IsGameSource = true,
            PlatformType = PlatformType.Pc,
            IsMpModeAvailable = IsMpModeAvailable
        };

        return proto;
    }
}
