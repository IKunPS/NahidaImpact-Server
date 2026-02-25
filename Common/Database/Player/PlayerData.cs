using NahidaImpact.Data;
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
    public int Level { get; set; } = 60;
    public int Exp { get; set; } = 0;
    public int BirthDay { get; set; } = 0;
    [SugarColumn(IsNullable = true)] public long LastActiveTime { get; set; }
    public long RegisterTime { get; set; } = Extensions.GetUnixSec();

    public static PlayerData? GetPlayerByUid(long uid)
    {
        var result = DatabaseHelper.GetInstance<PlayerData>((int)uid);
        return result;
    }
    
}
