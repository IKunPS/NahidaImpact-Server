using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneEntityDisappearNotify : BasePacket
{
    public PacketSceneEntityDisappearNotify(BaseEntity entity, VisionType visionType) : base(CmdIds.SceneEntityDisappearNotify)
    {
        var proto = new SceneEntityDisappearNotify()
        {
            DisappearType = visionType
        };
        
        proto.EntityList.Add(entity.Id);

        SetData(proto);
    }
}