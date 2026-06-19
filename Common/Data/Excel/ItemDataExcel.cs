using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NahidaImpact.Data.Common;
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
    [JsonPropertyName("itemUse")] public List<ItemUseActionData> ItemUseActions { get; set; } = [];

    // Destroy return materials (for material deletion refund)
    [JsonPropertyName("destroyReturnMaterial")] public List<int> DestroyReturnMaterial { get; set; } = [];
    [JsonPropertyName("destroyReturnMaterialCount")] public List<int> DestroyReturnMaterialCount { get; set; } = [];

    // Relic upgrade
    [JsonPropertyName("baseConvExp")] public int BaseConvExp { get; set; }
    [JsonPropertyName("addPropLevels")] public List<int> AddPropLevels { get; set; } = [];

    // Food
    [JsonPropertyName("satiationParams")] public List<int> SatiationParams { get; set; } = [];

    // Relic set
    [JsonPropertyName("setId")] public int SetId { get; set; }

    // Furniture
    [JsonPropertyName("comfort")] public int Comfort { get; set; }

    [JsonIgnore] public HashSet<int>? AddPropLevelSet { get; private set; }

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

    public MaterialType MaterialType => MaterialTypeStr switch
    {
        "MATERIAL_FOOD" => MaterialType.MATERIAL_FOOD,
        "MATERIAL_QUEST" => MaterialType.MATERIAL_QUEST,
        "MATERIAL_EXCHANGE" => MaterialType.MATERIAL_EXCHANGE,
        "MATERIAL_CONSUME" => MaterialType.MATERIAL_CONSUME,
        "MATERIAL_EXP_FRUIT" => MaterialType.MATERIAL_EXP_FRUIT,
        "MATERIAL_AVATAR" => MaterialType.MATERIAL_AVATAR,
        "MATERIAL_ADSORBATE" => MaterialType.MATERIAL_ADSORBATE,
        "MATERIAL_CRICKET" => MaterialType.MATERIAL_CRICKET,
        "MATERIAL_ELEM_CRYSTAL" => MaterialType.MATERIAL_ELEM_CRYSTAL,
        "MATERIAL_WEAPON_EXP_STONE" => MaterialType.MATERIAL_WEAPON_EXP_STONE,
        "MATERIAL_CHEST" => MaterialType.MATERIAL_CHEST,
        "MATERIAL_RELIQUARY_MATERIAL" => MaterialType.MATERIAL_RELIQUARY_MATERIAL,
        "MATERIAL_AVATAR_MATERIAL" => MaterialType.MATERIAL_AVATAR_MATERIAL,
        "MATERIAL_NOTICE_ADD_HP" => MaterialType.MATERIAL_NOTICE_ADD_HP,
        "MATERIAL_SEA_LAMP" => MaterialType.MATERIAL_SEA_LAMP,
        "MATERIAL_SELECTABLE_CHEST" => MaterialType.MATERIAL_SELECTABLE_CHEST,
        "MATERIAL_FLYCLOAK" => MaterialType.MATERIAL_FLYCLOAK,
        "MATERIAL_NAMECARD" => MaterialType.MATERIAL_NAMECARD,
        "MATERIAL_TALENT" => MaterialType.MATERIAL_TALENT,
        "MATERIAL_WIDGET" => MaterialType.MATERIAL_WIDGET,
        "MATERIAL_CHEST_BATCH_USE" => MaterialType.MATERIAL_CHEST_BATCH_USE,
        "MATERIAL_FAKE_ABSORBATE" => MaterialType.MATERIAL_FAKE_ABSORBATE,
        "MATERIAL_CONSUME_BATCH_USE" => MaterialType.MATERIAL_CONSUME_BATCH_USE,
        "MATERIAL_WOOD" => MaterialType.MATERIAL_WOOD,
        "MATERIAL_FURNITURE_FORMULA" => MaterialType.MATERIAL_FURNITURE_FORMULA,
        "MATERIAL_CHANNELLER_SLAB_BUFF" => MaterialType.MATERIAL_CHANNELLER_SLAB_BUFF,
        "MATERIAL_FURNITURE_SUITE_FORMULA" => MaterialType.MATERIAL_FURNITURE_SUITE_FORMULA,
        "MATERIAL_COSTUME" => MaterialType.MATERIAL_COSTUME,
        "MATERIAL_HOME_SEED" => MaterialType.MATERIAL_HOME_SEED,
        "MATERIAL_FISH_BAIT" => MaterialType.MATERIAL_FISH_BAIT,
        "MATERIAL_FISH_ROD" => MaterialType.MATERIAL_FISH_ROD,
        "MATERIAL_SUMO_BUFF" => MaterialType.MATERIAL_SUMO_BUFF,
        "MATERIAL_FIREWORKS" => MaterialType.MATERIAL_FIREWORKS,
        "MATERIAL_BGM" => MaterialType.MATERIAL_BGM,
        "MATERIAL_SPICE_FOOD" => MaterialType.MATERIAL_SPICE_FOOD,
        "MATERIAL_ACTIVITY_ROBOT" => MaterialType.MATERIAL_ACTIVITY_ROBOT,
        "MATERIAL_ACTIVITY_GEAR" => MaterialType.MATERIAL_ACTIVITY_GEAR,
        "MATERIAL_ACTIVITY_JIGSAW" => MaterialType.MATERIAL_ACTIVITY_JIGSAW,
        "MATERIAL_ARANARA" => MaterialType.MATERIAL_ARANARA,
        "MATERIAL_DESHRET_MANUAL" => MaterialType.MATERIAL_DESHRET_MANUAL,
        _ => MaterialType.MATERIAL_NONE
    };

    public bool IsEquip => ItemType is ItemType.ITEM_WEAPON or ItemType.ITEM_RELIQUARY;

    public bool CanAddRelicProp(int level) =>
        AddPropLevelSet != null && AddPropLevelSet.Contains(level);

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.ItemData[(int)Id] = this;

        if (ItemType == ItemType.ITEM_RELIQUARY && AddPropLevels.Count > 0)
            AddPropLevelSet = [..AddPropLevels];

        if (ItemType == ItemType.ITEM_WEAPON)
            EquipTypeStr = "EQUIP_WEAPON";
    }
}
