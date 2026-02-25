using NahidaImpact.GameServer.Game.Scene;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneEntityAppearNotify : BasePacket
{
    public PacketSceneEntityAppearNotify(SceneEntity entity, VisionType visionType) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = visionType,
            EntityList = { entity.ToProto() }
        };
        
        SetData(proto);
    }
}