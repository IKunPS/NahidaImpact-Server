using System.Collections.Generic;
using System.Linq;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketSceneEntityAppearNotify : BasePacket
{
    // Single entity, born vision type
    public PacketSceneEntityAppearNotify(BaseEntity entity) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = VisionType.VisionBorn,
            EntityList = { entity.ToProto() }
        };

        SetData(proto);
    }

    // Single entity with vision type
    public PacketSceneEntityAppearNotify(BaseEntity entity, VisionType visionType) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = visionType,
            EntityList = { entity.ToProto() },
        };

        SetData(proto);
    }

    // Single entity with vision type and param (mirrors Java: (GameEntity, VisionType, int))
    public PacketSceneEntityAppearNotify(BaseEntity entity, VisionType visionType, int param) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = visionType,
            Param = (uint)param,
            EntityList = { entity.ToProto() },
        };

        SetData(proto);
    }

    // Player constructor — delegates to current avatar entity (mirrors Java: (Player))
    public PacketSceneEntityAppearNotify(PlayerInstance player) : base(CmdIds.SceneEntityAppearNotify)
    {
        var currentAvatar = player.TeamManager?.GetCurrentAvatarEntity();
        var proto = new SceneEntityAppearNotify
        {
            AppearType = VisionType.VisionBorn,
        };

        if (currentAvatar != null)
        {
            proto.EntityList.Add(currentAvatar.ToProto());
        }

        SetData(proto);
    }

    // Batch constructor — multiple entities (mirrors Java: (Collection<GameEntity>, VisionType))
    public PacketSceneEntityAppearNotify(IEnumerable<BaseEntity> entities, VisionType visionType) : base(CmdIds.SceneEntityAppearNotify)
    {
        var proto = new SceneEntityAppearNotify
        {
            AppearType = visionType,
            EntityList = { entities.Select(e => e.ToProto()) }
        };

        SetData(proto);
    }
}
