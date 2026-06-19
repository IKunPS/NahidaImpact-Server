using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("ProudSkillExcelConfigData.json")]
public class ProudSkillDataExcel : ExcelResource
{
    [JsonPropertyName("proudSkillId")] public int ProudSkillId { get; set; }
    [JsonPropertyName("proudSkillGroupId")] public int ProudSkillGroupId { get; set; }
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("coinCost")] public int CoinCost { get; set; }
    [JsonPropertyName("breakLevel")] public int BreakLevel { get; set; }
    [JsonPropertyName("proudSkillType")] public int ProudSkillType { get; set; }
    [JsonPropertyName("openConfig")] public string OpenConfig { get; set; } = "";
    [JsonPropertyName("costItems")] public List<ItemParamData> CostItems { get; set; } = [];
    [JsonPropertyName("filterConds")] public List<string> FilterConds { get; set; } = [];
    [JsonPropertyName("lifeEffectParams")] public List<string> LifeEffectParams { get; set; } = [];
    [JsonPropertyName("addProps")] public FightPropData[] AddProps { get; set; } = [];
    [JsonPropertyName("paramList")] public float[] ParamList { get; set; } = [];
    [JsonPropertyName("paramDescList")] public long[] ParamDescList { get; set; } = [];
    [JsonPropertyName("nameTextMapHash")] public long NameTextMapHash { get; set; }

    public override uint GetId() => (uint)ProudSkillId;

    public override void Loaded()
    {
        GameData.ProudSkillData[ProudSkillId] = this;
    }

    public override void AfterAllDone()
    {
        // 过滤无效的addProps
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