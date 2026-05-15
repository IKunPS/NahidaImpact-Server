using System.Text.Json.Serialization;

namespace NahidaImpact.Data.Common;

public class PointData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("areaId")]
    public int AreaId { get; set; }

    [JsonPropertyName("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("tranPos")]
    public PointPosition? TranPos { get; set; }

    [JsonPropertyName("pos")]
    public PointPosition? Pos { get; set; }

    [JsonPropertyName("rot")]
    public PointPosition? Rot { get; set; }

    [JsonPropertyName("size")]
    public PointPosition? Size { get; set; }

    [JsonPropertyName("forbidSimpleUnlock")]
    public bool ForbidSimpleUnlock { get; set; }

    [JsonPropertyName("unlocked")]
    public bool Unlocked { get; set; }

    [JsonPropertyName("groupLimit")]
    public bool GroupLimit { get; set; }

    [JsonPropertyName("dungeonIds")]
    public int[]? DungeonIds { get; set; }

    [JsonPropertyName("dungeonRandomList")]
    public int[]? DungeonRandomList { get; set; }

    [JsonPropertyName("groupIDs")]
    public int[]? GroupIDs { get; set; }

    [JsonPropertyName("tranSceneId")]
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
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("z")]
    public float Z { get; set; }
}
