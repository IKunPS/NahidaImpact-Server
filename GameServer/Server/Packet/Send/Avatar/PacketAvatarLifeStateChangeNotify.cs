using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarLifeStateChangeNotify : BasePacket
{
    // Basic constructor — auto-detect state from avatar data
    public PacketAvatarLifeStateChangeNotify(AvatarDataInfo avatar) : base(CmdIds.AvatarLifeStateChangeNotify)
    {
        var proto = new AvatarLifeStateChangeNotify
        {
            AvatarGuid = avatar.Guid,
            LifeState = avatar.LifeState
        };

        SetData(proto);
    }

    // Constructor with attacker and life state (mirrors Java: (Avatar, int attackerId, LifeState))
    public PacketAvatarLifeStateChangeNotify(AvatarDataInfo avatar, uint attackerId, uint lifeState) : base(CmdIds.AvatarLifeStateChangeNotify)
    {
        var proto = new AvatarLifeStateChangeNotify
        {
            AvatarGuid = avatar.Guid,
            LifeState = lifeState
        };

        SetData(proto);
    }

    // Full constructor (mirrors Java: (Avatar, LifeState, GameEntity, String, PlayerDieType))
    public PacketAvatarLifeStateChangeNotify(AvatarDataInfo avatar, uint lifeState, uint sourceEntityId, string attackTag, uint dieType) : base(CmdIds.AvatarLifeStateChangeNotify)
    {
        var proto = new AvatarLifeStateChangeNotify
        {
            AvatarGuid = avatar.Guid,
            LifeState = lifeState
        };

        SetData(proto);
    }
}
