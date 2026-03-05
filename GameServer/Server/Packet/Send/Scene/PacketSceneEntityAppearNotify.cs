using System;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneEntityAppearNotify : BasePacket
{
    public PacketSceneEntityAppearNotify(BaseEntity entity): base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = VisionType.VisionBorn,
            EntityList = { entity.ToProto() }
        };

        SetData(proto);
    }
    
    public PacketSceneEntityAppearNotify(BaseEntity entity, VisionType visionType) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = visionType,
            EntityList = { entity.ToProto() },
        };

        SetData(proto);
    }
}