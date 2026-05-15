using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.MapMarks;

public class MapMark
{
    public int SceneId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Position? Position { get; set; }
    public MapMarkPointType PointType { get; set; }
    public int MonsterId { get; set; }
    public MapMarkFromType FromType { get; set; }
    public int QuestId { get; set; }

    public MapMark() { }

    public MapMark(MapMarkPoint mapMarkPoint)
    {
        SceneId = (int)mapMarkPoint.SceneId;
        Name = mapMarkPoint.Name;
        if (mapMarkPoint.Pos != null)
            Position = new Position(mapMarkPoint.Pos.X, mapMarkPoint.Pos.Y, mapMarkPoint.Pos.Z);
        PointType = mapMarkPoint.PointType;
        MonsterId = (int)mapMarkPoint.MonsterId;
        FromType = mapMarkPoint.FromType;
        QuestId = (int)mapMarkPoint.QuestId;
    }
}
