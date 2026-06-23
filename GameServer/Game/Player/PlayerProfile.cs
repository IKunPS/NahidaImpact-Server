using NahidaImpact.Data.Excel;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerProfile
{
    public uint NameCardId { get; set; } = ConstValue.GetUint("CONST_VALUE_DEFAULT_NAME_CARD_ID");
    public string? Nickname { get; set; }
    public uint Level { get; set; } = 1;
    public uint WorldLevel { get; set; } = 0;
    public Birthday Birthday { get; set; } = new() { Day = 0, Month = 0 };
    public ProfilePicture HeadImage { get; set; } = new() { AvatarId = GameConstants.MAIN_CHARACTER_FEMALE };
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
