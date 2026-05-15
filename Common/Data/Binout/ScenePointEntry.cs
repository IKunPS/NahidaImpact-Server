using NahidaImpact.Data.Common;

namespace NahidaImpact.Data.Binout;

public class ScenePointEntry
{
    public int SceneId { get; }
    public PointData PointData { get; }

    public ScenePointEntry(int sceneId, PointData pointData)
    {
        SceneId = sceneId;
        PointData = pointData;
    }

    public string GetName() => $"{SceneId}_{PointData.Id}";
}
