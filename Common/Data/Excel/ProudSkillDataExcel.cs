using Newtonsoft.Json;
using NahidaImpact.Data.Common;
using NahidaImpact.Prop;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("ProudSkillExcelConfigData.json")]
public class ProudSkillDataExcel : ExcelResource
{
    [JsonProperty("proudSkillId")] public int ProudSkillId { get; set; }
    [JsonProperty("proudSkillGroupId")] public int ProudSkillGroupId { get; set; }
    [JsonProperty("level")] public int Level { get; set; }
    [JsonProperty("coinCost")] public int CoinCost { get; set; }
    [JsonProperty("breakLevel")] public int BreakLevel { get; set; }
    [JsonProperty("proudSkillType")] public int ProudSkillType { get; set; }
    [JsonProperty("openConfig")] public string OpenConfig { get; set; } = "";
    [JsonProperty("costItems")] public List<ItemParamData> CostItems { get; set; } = [];
    [JsonProperty("filterConds")] public List<string> FilterConds { get; set; } = [];
    [JsonProperty("lifeEffectParams")] public List<string> LifeEffectParams { get; set; } = [];
    [JsonProperty("addProps")] public FightPropData[] AddProps { get; set; } = [];
    [JsonProperty("paramList")] public float[] ParamList { get; set; } = [];
    [JsonProperty("paramDescList")] public long[] ParamDescList { get; set; } = [];
    [JsonProperty("nameTextMapHash")] public long NameTextMapHash { get; set; }

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