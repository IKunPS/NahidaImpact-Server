using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NahidaImpact.Enums.Player;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("OpenStateConfigData.json")]
public class OpenStateDataExcel : ExcelResource
{
    [JsonPropertyName("id")] public uint Id;
    
    [JsonPropertyName("defaultState")] public bool DefaultState { get; set; }
    
    [JsonPropertyName("allowClientOpen")] public bool AllowClientOpen { get; set; }
    
    [JsonPropertyName("systemOpenUiId")] public int SystemOpenUiId;
    
    [JsonPropertyName("cond")] public List<OpenStateCond> Cond { get; set; } = new();
    
    public override uint GetId()
    {
        return Id;
    }
    
    public override void Loaded()
    {
        // Remove any empty conditions
        if (Cond != null)
        {
            Cond.RemoveAll(c => c == null);
        }
        else
        {
            Cond = new List<OpenStateCond>();
        }
        
        GameData.OpenStateData.Add((int)Id, this);   
    }
}

public class OpenStateCond
{
    [JsonPropertyName("condType")] public OpenStateCondType CondType { get; set; }
    [JsonPropertyName("param")] public int Param { get; set; }
    [JsonPropertyName("param2")] public int Param2 { get; set; }
}

