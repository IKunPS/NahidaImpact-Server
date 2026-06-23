using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("AvatarCostumeExcelConfigData.json")]
public class CostumeDataExcel : ExcelResource
{
    [JsonProperty("skinId")]
    public uint SkinId { get; set; }

    [JsonProperty("characterId")]
    public uint CharacterId { get; set; }

    [JsonProperty("itemId")]
    public uint ItemId { get; set; }

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }

    [JsonProperty("hide")]
    public bool Hide { get; set; }

    [JsonProperty("indexID")]
    public uint IndexId { get; set; }

    [JsonProperty("nameTextMapHash")]
    public uint NameTextMapHash { get; set; }

    [JsonProperty("quality")]
    public uint Quality { get; set; }

    public override uint GetId() => SkinId;

    public override void Loaded()
    {
        GameData.CostumeData[(int)SkinId] = this;
    }
}
