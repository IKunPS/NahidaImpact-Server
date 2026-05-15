using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketScenePointUnlockNotify : BasePacket
{
    public PacketScenePointUnlockNotify(int sceneId, int pointId) : base(CmdIds.ScenePointUnlockNotify)
    {
        var proto = new ScenePointUnlockNotify
        {
            SceneId = (uint)sceneId
        };
        proto.PointList.Add((uint)pointId);
        SetData(proto);
    }

    public PacketScenePointUnlockNotify(int sceneId, IEnumerable<int> pointIds) : base(CmdIds.ScenePointUnlockNotify)
    {
        var pointList = pointIds.Select(p => (uint)p).ToList();
        var proto = new ScenePointUnlockNotify
        {
            SceneId = (uint)sceneId
        };
        proto.PointList.AddRange(pointList);

        SetData(proto);
    }
}
