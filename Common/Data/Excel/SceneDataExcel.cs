using System.Collections.Generic;
using System.Text.Json.Serialization;
using NahidaImpact.Enums.Scene;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("SceneExcelConfigData.json")]
public class SceneDataExcel : ExcelResource
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }
    
    [JsonPropertyName("type")]
    public SceneTypeEnum SceneType { get; set; }
    
    [JsonPropertyName("scriptData")]
    public string ScriptData { get; set; } = string.Empty;
    
    [JsonPropertyName("levelEntityConfig")]
    public string LevelEntityConfig { get; set; } = string.Empty;
    
    [JsonPropertyName("specifiedAvatarList")]
    public List<int> SpecifiedAvatarList { get; set; } = [];
    
    public override uint GetId()
    {
        return Id;
    }
    
    public override void Loaded()
    {
        GameData.SceneData.Add((int)Id, this);
    }
}