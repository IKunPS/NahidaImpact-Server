using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Entity;

public class PacketSceneEntityMoveNotify : BasePacket
{
    public PacketSceneEntityMoveNotify(EntityMoveInfo moveInfo) : base(CmdIds.SceneEntityMoveNotify)
    {
        var notify = new SceneEntityMoveNotify
        {
            MotionInfo = moveInfo.MotionInfo,
            EntityId = moveInfo.EntityId,
            SceneTime = moveInfo.SceneTime,
            ReliableSeq = moveInfo.ReliableSeq
        };

        SetData(notify);
    }
}
