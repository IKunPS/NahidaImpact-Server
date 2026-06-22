using Newtonsoft.Json;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarTalentExcelConfigData.json")]
public class AvatarTalentDataExcel : ExcelResource
{
    [JsonProperty("talentId")] public int TalentId { get; set; }
    [JsonProperty("prevTalent")] public int PrevTalent { get; set; }
    [JsonProperty("nameTextMapHash")] public long NameTextMapHash { get; set; }
    [JsonProperty("icon")] public string Icon { get; set; } = "";
    [JsonProperty("mainCostItemId")] public int MainCostItemId { get; set; }
    [JsonProperty("mainCostItemCount")] public int MainCostItemCount { get; set; }
    [JsonProperty("openConfig")] public string OpenConfig { get; set; } = "";
    [JsonProperty("addProps")] public FightPropData[] AddProps { get; set; } = [];
    [JsonProperty("paramList")] public float[] ParamList { get; set; } = [];

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