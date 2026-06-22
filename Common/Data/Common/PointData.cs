using Newtonsoft.Json;

namespace NahidaImpact.Data.Common;

public class PointData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("areaId")]
    public int AreaId { get; set; }

    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("tranPos")]
    public PointPosition? TranPos { get; set; }

    [JsonProperty("pos")]
    public PointPosition? Pos { get; set; }

    [JsonProperty("rot")]
    public PointPosition? Rot { get; set; }

    [JsonProperty("size")]
    public PointPosition? Size { get; set; }

    [JsonProperty("forbidSimpleUnlock")]
    public bool ForbidSimpleUnlock { get; set; }

    [JsonProperty("unlocked")]
    public bool Unlocked { get; set; }

    [JsonProperty("groupLimit")]
    public bool GroupLimit { get; set; }

    [JsonProperty("dungeonIds")]
    public int[]? DungeonIds { get; set; }

    [JsonProperty("dungeonRandomList")]
    public int[]? DungeonRandomList { get; set; }

    [JsonProperty("groupIDs")]
    public int[]? GroupIDs { get; set; }

    [JsonProperty("tranSceneId")]
    public int TranSceneId { get; set; }

    /// <summary>
    /// Update dungeon IDs based on the daily rotation.
    /// Mirrors Java PointData.updateDailyDungeon().
    /// </summary>
    public void UpdateDailyDungeon()
    {
        if (DungeonRandomList == null || DungeonRandomList.Length == 0)
            return;

        var newDungeons = new List<int>();
        int day = (int)DateTime.Now.DayOfWeek;

        foreach (var randomId in DungeonRandomList)
        {
            // TODO: Get DailyDungeonData from GameData and resolve dungeons by day
            // var data = GameData.GetDailyDungeonDataMap().GetValueOrDefault(randomId);
            // if (data != null) newDungeons.AddRange(data.GetDungeonsByDay(day));
        }

        // DungeonIds = newDungeons.ToArray();
    }
}

/// <summary>
/// Position data for scene points. Lives in Common so it doesn't depend on GameServer's Position class.
/// </summary>
public class PointPosition
{
    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("z")]
    public float Z { get; set; }
}
