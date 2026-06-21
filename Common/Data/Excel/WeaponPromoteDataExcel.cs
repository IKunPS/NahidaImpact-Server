using Newtonsoft.Json;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("WeaponPromoteExcelConfigData.json")]
public class WeaponPromoteDataExcel : ExcelResource
{
    [JsonProperty("weaponPromoteId")]
    public int WeaponPromoteId { get; set; }

    [JsonProperty("promoteLevel")]
    public int PromoteLevel { get; set; }

    [JsonProperty("coinCost")]
    public int CoinCost { get; set; }

    [JsonProperty("costItems")]
    public List<ItemParamData> CostItems { get; set; } = [];

    [JsonProperty("addProps")]
    public FightPropData[] AddProps { get; set; } = [];

    [JsonProperty("unlockMaxLevel")]
    public int UnlockMaxLevel { get; set; }

    [JsonProperty("requiredPlayerLevel")]
    public int RequiredPlayerLevel { get; set; }

    public override uint GetId() => (uint)WeaponPromoteId;

    public override void Loaded()
    {
        var key = (WeaponPromoteId << 8) + PromoteLevel;
        GameData.WeaponPromoteData[key] = this;
    }

    public override void AfterAllDone()
    {
        var parsed = new List<FightPropData>();
        foreach (var prop in AddProps)
        {
            if (prop.PropType != null && prop.Value != 0f)
            {
                prop.OnLoad();
                parsed.Add(prop);
            }
        }
        AddProps = parsed.ToArray();
    }
}