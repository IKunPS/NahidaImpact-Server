using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Player;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Player;

public class PlayerProfile
{
    public uint NameCardId { get; set; } = ConstValue.GetUint("CONST_VALUE_DEFAULT_NAME_CARD_ID");
    public string Nickname { get; set; }
    public uint Level { get; set; } = 1;
    public uint WorldLevel { get; set; } = 0;
    public Birthday Birthday { get; set; } = new() { Day = 0, Month = 0 };
    public ProfilePicture HeadImage { get; set; } = new();
    public string Signature { get; set; } = "";
    public uint Achievements { get; set; }
    public uint DaysSinceLastLogin { get; set; }
    public long LastActiveTime { get; set; }

    public PlayerProfile(string nickname)
    {
        Nickname = nickname;
    }

    // Load runtime state from persisted PlayerData
    public void LoadFromData(PlayerData data)
    {
        Nickname = data.Name ?? Nickname;
        Signature = data.Signature ?? "";
        WorldLevel = (uint)data.WorldLevel;
        NameCardId = (uint)data.NameCardId;
        HeadImage = new ProfilePicture
        {
            ProfilePictureId = data.ProfilePictureAvatarId,
            CostumeId = data.ProfilePictureCostumeId
        };
        if (data.BirthDay > 0)
            Birthday = new Birthday { Month = data.GetBirthdayMonth(), Day = data.GetBirthdayDay() };
        LastActiveTime = data.LastActiveTime;
    }

    // Sync mutable profile fields back to PlayerData for persistence
    public void SyncToData(PlayerData data)
    {
        data.Signature = Signature;
        data.NameCardId = (int)NameCardId;
        data.ProfilePictureAvatarId = HeadImage?.AvatarId ?? 0;
        data.ProfilePictureCostumeId = HeadImage?.CostumeId ?? 0;
        data.SetBirthday(Birthday?.Month ?? 0, Birthday?.Day ?? 0);
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
