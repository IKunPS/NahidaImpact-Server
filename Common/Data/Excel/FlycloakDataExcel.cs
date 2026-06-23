using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarFlycloakExcelConfigData.json")]
public class FlycloakDataExcel : ExcelResource
{
    [JsonProperty("flycloakId")]
    public uint FlycloakId { get; set; }

    [JsonProperty("nameTextMapHash")]
    public uint NameTextMapHash { get; set; }

    [JsonProperty("descTextMapHash")]
    public uint DescTextMapHash { get; set; }

    [JsonProperty("hide")]
    public bool Hide { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; } = "";

    [JsonProperty("jsonName")]
    public string JsonName { get; set; } = "";

    [JsonProperty("prefabPath")]
    public string PrefabPath { get; set; } = "";

    public override uint GetId() => FlycloakId;

    public override void Loaded()
    {
        GameData.FlycloakData[(int)FlycloakId] = this;
    }
}
