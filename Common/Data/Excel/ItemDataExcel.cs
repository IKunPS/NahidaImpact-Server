using System.Text.Json.Serialization;
using NahidaImpact.Enums.Item;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("MaterialExcelConfigData.json")]
public class ItemDataExcel : ExcelResource
{
    [JsonPropertyName("id")] public uint Id { get; set; }
    [JsonPropertyName("itemType")] public string ItemTypeStr { get; set; } = "ITEM_NONE";
    [JsonPropertyName("materialType")] public string MaterialTypeStr { get; set; } = "MATERIAL_NONE";
    [JsonPropertyName("stackLimit")] public uint StackLimit { get; set; } = 99999;
    [JsonPropertyName("useOnGain")] public bool UseOnGain { get; set; }
    [JsonPropertyName("useTarget")] public string UseTarget { get; set; } = "";
    [JsonPropertyName("gadgetId")] public uint GadgetId { get; set; }
    [JsonPropertyName("equipType")] public string EquipTypeStr { get; set; } = "EQUIP_NONE";
    [JsonPropertyName("icon")] public string Icon { get; set; } = "";
    [JsonPropertyName("rankLevel")] public uint RankLevel { get; set; }
    [JsonPropertyName("weaponPromoteId")] public uint WeaponPromoteId { get; set; }
    [JsonPropertyName("weaponBaseExp")] public uint WeaponBaseExp { get; set; }
    [JsonPropertyName("skillAffix")] public List<int> SkillAffix { get; set; } = [];
    [JsonPropertyName("mainPropDepotId")] public int MainPropDepotId { get; set; }
    [JsonPropertyName("appendPropDepotId")] public int AppendPropDepotId { get; set; }
    [JsonPropertyName("appendPropNum")] public int AppendPropNum { get; set; }
    [JsonPropertyName("maxLevel")] public int MaxLevel { get; set; } = 90;
    [JsonPropertyName("awakenMaterial")] public uint AwakenMaterial { get; set; }
    [JsonPropertyName("awakenCosts")] public List<uint> AwakenCosts { get; set; } = [];

    public ItemType ItemType => ItemTypeStr switch
    {
        "ITEM_VIRTUAL" => ItemType.ITEM_VIRTUAL,
        "ITEM_MATERIAL" => ItemType.ITEM_MATERIAL,
        "ITEM_RELIQUARY" => ItemType.ITEM_RELIQUARY,
        "ITEM_WEAPON" => ItemType.ITEM_WEAPON,
        "ITEM_DISPLAY" => ItemType.ITEM_DISPLAY,
        "ITEM_FURNITURE" => ItemType.ITEM_FURNITURE,
        _ => ItemType.ITEM_NONE
    };

    public bool IsEquip => ItemType is ItemType.ITEM_WEAPON or ItemType.ITEM_RELIQUARY;

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.ItemData[(int)Id] = this;
    }
}
