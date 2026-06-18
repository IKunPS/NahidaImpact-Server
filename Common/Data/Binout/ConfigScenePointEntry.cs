using NahidaImpact.Data.Common;

namespace NahidaImpact.Data.Binout;

public class ConfigScenePointEntry
{
    public int SceneId { get; }
    public PointData PointData { get; }

    public ConfigScenePointEntry(int sceneId, PointData pointData)
    {
        SceneId = sceneId;
        PointData = pointData;
    }
}
