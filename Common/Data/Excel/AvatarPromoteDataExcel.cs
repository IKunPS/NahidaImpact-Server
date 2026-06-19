using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarPromoteExcelConfigData.json")]
public class AvatarPromoteDataExcel : ExcelResource
{
    [JsonPropertyName("avatarPromoteId")] public int AvatarPromoteId { get; set; }
    [JsonPropertyName("promoteLevel")] public int PromoteLevel { get; set; }
    [JsonPropertyName("scoinCost")] public int CoinCost { get; set; }
    [JsonPropertyName("costItems")] public List<ItemParamData> CostItems { get; set; } = [];
    [JsonPropertyName("addProps")] public List<FightPropData> AddProps { get; set; } = [];
    [JsonPropertyName("unlockMaxLevel")] public int UnlockMaxLevel { get; set; }
    [JsonPropertyName("requiredPlayerLevel")] public int RequiredPlayerLevel { get; set; }

    public override uint GetId() => (uint)AvatarPromoteId;

    public override void Loaded()
    {
        var key = (AvatarPromoteId << 8) + PromoteLevel;
        GameData.AvatarPromoteData[key] = this;
    }

    public override void AfterAllDone()
    {
        // Filter out empty prop entries
        AddProps = AddProps.Where(p => !string.IsNullOrEmpty(p.PropType) && p.Value != 0f).ToList();
        CostItems = CostItems.Where(c => c.Id > 0).ToList();
    }
}
