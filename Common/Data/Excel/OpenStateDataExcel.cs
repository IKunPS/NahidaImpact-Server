using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using NahidaImpact.Enums.Player;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("OpenStateConfigData.json")]
public class OpenStateDataExcel : ExcelResource
{
    [JsonProperty("id")] public uint Id;
    
    [JsonProperty("defaultState")] public bool DefaultState { get; set; }
    
    [JsonProperty("allowClientOpen")] public bool AllowClientOpen { get; set; }
    
    [JsonProperty("systemOpenUiId")] public int SystemOpenUiId;
    
    [JsonProperty("cond")] public List<OpenStateCond> Cond { get; set; } = new();
    
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
    [JsonProperty("condType")] public OpenStateCondType CondType { get; set; }
    [JsonProperty("param")] public int Param { get; set; }
    [JsonProperty("param2")] public int Param2 { get; set; }
}

