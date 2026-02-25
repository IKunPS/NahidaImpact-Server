namespace NahidaImpact.GameServer.Game.Player;

public class PlayerProfile
{
    public uint NameCardId { get; set; } = 210001;
    public string? Nickname { get; set; }
    public uint Level { get; set; } = 1;
    public uint WorldLevel { get; set; } = 0;
    public Birthday Birthday { get; set; } = new() { Day = 0, Month = 0 };
    public ProfilePicture HeadImage { get; set; } = new() { AvatarId = 10000007 };
    public string? Signature { get; set; } = "";
    public uint Achievements { get; set; }
    public uint DaysSinceLastLogin { get; set; }
    public long LastActiveTime { get; set; }

    public PlayerProfile(string nickname)
    {
        Nickname = nickname;
    }

    public FriendBrief ToFriendBrief(uint uid, bool isOnline = false)
    {
        return new FriendBrief
        {
            Uid = uid,
            Nickname = Nickname ?? "",
            Level = Level,
            ProfilePicture = HeadImage,
            WorldLevel = WorldLevel,
            Signature = Signature ?? "",
            OnlineState = isOnline ? FriendOnlineState.FriendOnline : FriendOnlineState.FreiendDisconnect,
            LastActiveTime = (uint)LastActiveTime,
            NameCardId = NameCardId,
            Param = DaysSinceLastLogin,
            IsGameSource = true,
            PlatformType = PlatformType.Pc
        };
    }
}
