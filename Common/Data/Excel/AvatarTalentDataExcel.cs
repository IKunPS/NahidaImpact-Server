using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarTalentExcelConfigData.json")]
public class AvatarTalentDataExcel : ExcelResource
{
    [JsonPropertyName("talentId")] public int TalentId { get; set; }
    [JsonPropertyName("prevTalent")] public int PrevTalent { get; set; }
    [JsonPropertyName("nameTextMapHash")] public long NameTextMapHash { get; set; }
    [JsonPropertyName("icon")] public string Icon { get; set; } = "";
    [JsonPropertyName("mainCostItemId")] public int MainCostItemId { get; set; }
    [JsonPropertyName("mainCostItemCount")] public int MainCostItemCount { get; set; }
    [JsonPropertyName("openConfig")] public string OpenConfig { get; set; } = "";
    [JsonPropertyName("addProps")] public FightPropData[] AddProps { get; set; } = [];
    [JsonPropertyName("paramList")] public float[] ParamList { get; set; } = [];

    public override uint GetId() => (uint)TalentId;

    public override void Loaded()
    {
        GameData.AvatarTalentData[TalentId] = this;
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