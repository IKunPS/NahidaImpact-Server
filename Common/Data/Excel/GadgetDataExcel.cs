using Newtonsoft.Json;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("GadgetExcelConfigData.json")]
public class GadgetDataExcel : ExcelResource
{
    [JsonProperty("id")] public uint Id { get; set; }
    [JsonProperty("jsonName")] public string JsonName { get; set; } = "";
    [JsonProperty("type")] public string GadgetTypeStr { get; set; } = "";
    [JsonProperty("tags")] public List<string> Tags { get; set; } = [];
    [JsonProperty("campID")] public int CampId { get; set; }
    [JsonProperty("isInteractive")] public bool IsInteractive { get; set; }
    [JsonProperty("isEquip")] public bool IsEquip { get; set; }
    [JsonProperty("hasMove")] public bool HasMove { get; set; }
    [JsonProperty("hasAudio")] public bool HasAudio { get; set; }
    [JsonProperty("itemJsonName")] public string ItemJsonName { get; set; } = "";
    [JsonProperty("visionLevel")] public string VisionLevel { get; set; } = "VISION_LEVEL_NORMAL";
    [JsonProperty("radarHintID")] public int RadarHintId { get; set; }
    [JsonProperty("chainId")] public int ChainId { get; set; }
    [JsonProperty("mpPropID")] public int MpPropId { get; set; }
    [JsonProperty("landSoundID")] public int LandSoundId { get; set; }
    [JsonProperty("nameTextMapHash")] public long NameTextMapHash { get; set; }
    [JsonProperty("interactNameTextMapHash")] public long InteractNameTextMapHash { get; set; }
    [JsonProperty("prefabPathHash")] public ulong PrefabPathHash { get; set; }
    [JsonProperty("prefabPathRemoteHash")] public ulong PrefabPathRemoteHash { get; set; }
    [JsonProperty("controllerPathHash")] public ulong ControllerPathHash { get; set; }
    [JsonProperty("controllerPathRemoteHash")] public ulong ControllerPathRemoteHash { get; set; }
    [JsonProperty("clientScriptHash")] public ulong ClientScriptHash { get; set; }
    [JsonProperty("inteeIconName")] public string InteeIconName { get; set; } = "";
    [JsonProperty("lodPatternName")] public string LodPatternName { get; set; } = "";

    // Obfuscated fields
    [JsonProperty("CAHOOGLFIEO")] public bool CAHOOGLFIEO { get; set; }
    [JsonProperty("GAKEGAPNKGA")] public bool GAKEGAPNKGA { get; set; }
    [JsonProperty("GPOBDDJGDEL")] public bool GPOBDDJGDEL { get; set; }
    [JsonProperty("MNPMIIGKLNL")] public bool MNPMIIGKLNL { get; set; }
    [JsonProperty("GIDMHPDBNNH")] public string GIDMHPDBNNH { get; set; } = "None";

    // hk4e GadgetType enum mapping
    public GadgetType Type => GadgetTypeStr switch
    {
        "Gear" => GadgetType.Gear,
        "Chest" => GadgetType.Chest,
        "Bulletin" => GadgetType.Bulletin,
        "ActionPoint" => GadgetType.ActionPoint,
        "RewardPoint" => GadgetType.RewardPoint,
        "Statue" => GadgetType.Statue,
        "WeeklyBossResin" => GadgetType.WeeklyBossResin,
        "Challenge" => GadgetType.Challenge,
        "GatherPoint" => GadgetType.GatherPoint,
        "GatherObject" => GadgetType.GatherObject,
        "Worktop" => GadgetType.Worktop,
        "CoolDown" => GadgetType.CoolDown,
        "ChestLocked" => GadgetType.ChestLocked,
        "EnvAnimal" => GadgetType.EnvAnimal,
        "Explore" => GadgetType.Explore,
        "FishPool" => GadgetType.FishPool,
        "CustomChest" => GadgetType.CustomChest,
        "Crystal" => GadgetType.Crystal,
        "MusicGame" => GadgetType.MusicGame,
        "Rouge" => GadgetType.Rouge,
        "BlossomChest" => GadgetType.BlossomChest,
        "DeshretObelisk" => GadgetType.DeshretObelisk,
        "CoinCollect" => GadgetType.CoinCollect,
        "EchoShell" => GadgetType.EchoShell,
        "Foundation" => GadgetType.Foundation,
        "ActivityInteract" => GadgetType.ActivityInteract,
        "CustomGadget" => GadgetType.CustomGadget,
        _ => GadgetType.None
    };

    public override uint GetId() => Id;

    public override void Loaded()
    {
        GameData.GadgetData[(int)Id] = this;
    }
}

// hk4e GadgetType — maps from GadgetExcelConfigData.json "type" field
public enum GadgetType
{
    None,
    Gear,
    Chest,
    Bulletin,
    ActionPoint,
    RewardPoint,
    Statue,
    WeeklyBossResin,
    Challenge,
    GatherPoint,
    GatherObject,
    Worktop,
    CoolDown,
    ChestLocked,
    EnvAnimal,
    Explore,
    FishPool,
    CustomChest,
    Crystal,
    MusicGame,
    Rouge,
    BlossomChest,
    DeshretObelisk,
    CoinCollect,
    EchoShell,
    Foundation,
    ActivityInteract,
    CustomGadget,
}
