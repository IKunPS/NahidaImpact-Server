using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

/// <summary>
/// Avatar skill config. Mirrors Java AvatarSkillData.
/// Loaded from AvatarSkillExcelConfigData.json.
/// </summary>
[ResourceEntity("AvatarSkillExcelConfigData.json")]
public class AvatarSkillDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    /// <summary>Element type as string (e.g. "Fire", "Electric"). Converted via ElementTypeValue.</summary>
    [JsonProperty("costElemType")]
    public string CostElemType { get; set; } = "";

    [JsonProperty("costElemVal")]
    public float CostElemVal { get; set; }

    [JsonProperty("maxChargeNum")]
    public int MaxChargeNum { get; set; }

    [JsonProperty("triggerID")]
    public uint TriggerId { get; set; }

    [JsonProperty("cdTime")]
    public float CdTime { get; set; }

    [JsonProperty("proudSkillGroupId")]
    public int ProudSkillGroupId { get; set; }

    /// <summary>Parsed element type: 1=Fire, 2=Water, 3=Wind, 4=Electric, 5=Grass, 6=Ice, 7=Rock.</summary>
    public int ElementTypeValue => CostElemType switch
    {
        "Fire" => 1,
        "Water" => 2,
        "Wind" => 3,
        "Electric" => 4,
        "Grass" => 5,
        "Ice" => 6,
        "Rock" => 7,
        _ => 0
    };

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.AvatarSkillData[(int)Id] = this;
    }
}