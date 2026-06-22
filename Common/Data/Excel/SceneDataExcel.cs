using Newtonsoft.Json;
using System.Collections.Generic;
using NahidaImpact.Enums.Scene;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("SceneExcelConfigData.json")]
public class SceneDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }
    
    [JsonProperty("type")]
    public SceneTypeEnum SceneType { get; set; }
    
    [JsonProperty("scriptData")]
    public string ScriptData { get; set; } = string.Empty;
    
    [JsonProperty("levelEntityConfig")]
    public string LevelEntityConfig { get; set; } = string.Empty;
    
    [JsonProperty("specifiedAvatarList")]
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