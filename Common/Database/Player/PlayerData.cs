using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Proto;
using NahidaImpact.Util;
using NahidaImpact.Util.Extensions;
using SqlSugar;

namespace NahidaImpact.Database.Player;

[SugarTable("Player")]
public class PlayerData : BaseDatabaseDataHelper
{
    public string? Name { get; set; } = "";
    public string? Signature { get; set; } = "NahidaPS";
    public int WorldLevel { get; set; } = 1;
    public int Exp { get; set; } = 0;
    public int BirthDay { get; set; } = 0;
    [SugarColumn(IsNullable = true)] public long LastActiveTime { get; set; }
    public long RegisterTime { get; set; } = Extensions.UnixSec;
    public int NameCardId { get; set; }
    public uint ProfilePictureAvatarId { get; set; }
    public uint ProfilePictureCostumeId { get; set; }

    [SugarColumn(IsJson = true)] public List<int> FlyCloakList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<int> NameCardList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<int> CostumeList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<int> TraceEffectList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<uint> ChatEmojiIdList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<uint> ShowAvatarIdList { get; set; } = [];
    [SugarColumn(IsJson = true)] public List<uint> ShowNameCardIdList { get; set; } = [];

    // Packed birthday: month * 100 + day; 0 = unset
    public void SetBirthday(uint month, uint day) => BirthDay = (int)(month * 100 + day);
    public uint GetBirthdayMonth() => (uint)(BirthDay / 100);
    public uint GetBirthdayDay() => (uint)(BirthDay % 100);

    public static PlayerData? GetPlayerByUid(long uid)
    {
        var result = DatabaseHelper.GetInstance<PlayerData>((int)uid);
        return result;
    }

    public static PlayerData GetOrCreatePlayerData(int uid)
    {
        return DatabaseHelper.GetInstanceOrCreateNew<PlayerData>(uid);
    }

    public static void SavePlayerData(PlayerData data)
    {
        DatabaseHelper.UpdateInstance(data);
    }

    // Instance save helper for convenience
    public void Save() => MarkForSave();

    public void MarkForSave()
    {
        DatabaseHelper.NeedSave(Uid);
    }
}