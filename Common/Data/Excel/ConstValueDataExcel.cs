using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("ConstValueExcelConfigData.json")]
public class ConstValueDataExcel : ExcelResource
{
    [JsonProperty("name")] public string Name { get; set; } = "";
    [JsonProperty("value")] public List<string> Value { get; set; } = [];

    public override uint GetId() => 0;

    public override void Loaded()
    {
        GameData.ConstValueMap[Name] = this;
    }

    public int GetInt(int index = 0)
        => Value.Count > index && int.TryParse(Value[index], out var v) ? v : 0;
}

public static class ConstValue
{
    public static uint GetUint(string name) =>
        GameData.ConstValueMap.TryGetValue(name, out var v) ? (uint)v.GetInt() : 0u;

    public static int GetInt(string name) =>
        GameData.ConstValueMap.TryGetValue(name, out var v) ? v.GetInt() : 0;
}
