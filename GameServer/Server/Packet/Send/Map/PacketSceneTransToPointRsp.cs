using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketSceneTransToPointRsp : BasePacket
{
    /// <summary>
    /// Success response.
    /// </summary>
    public PacketSceneTransToPointRsp(PlayerInstance player, int pointId, int sceneId) : base(CmdIds.SceneTransToPointRsp)
    {
        var proto = new SceneTransToPointRsp
        {
            Retcode = 0,
            PointId = (uint)pointId,
            SceneId = (uint)sceneId
        };

        SetData(proto);
    }

    /// <summary>
    /// Error response.
    /// </summary>
    public PacketSceneTransToPointRsp() : base(CmdIds.SceneTransToPointRsp)
    {
        var proto = new SceneTransToPointRsp
        {
            Retcode = -1 // RET_SVR_ERROR equivalent
        };

        SetData(proto);
    }
}
