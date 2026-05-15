using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketSceneAreaUnlockNotify : BasePacket
{
    public PacketSceneAreaUnlockNotify(int sceneId, int areaId) : base(CmdIds.SceneAreaUnlockNotify)
    {
        var proto = new SceneAreaUnlockNotify
        {
            SceneId = (uint)sceneId
        };
        proto.AreaList.Add((uint)areaId);

        SetData(proto);
    }

    public PacketSceneAreaUnlockNotify(int sceneId, IEnumerable<int> areaIds) : base(CmdIds.SceneAreaUnlockNotify)
    {
        var proto = new SceneAreaUnlockNotify
        {
            SceneId = (uint)sceneId
        };
        proto.AreaList.AddRange(areaIds.Select(a => (uint)a));

        SetData(proto);
    }
}
