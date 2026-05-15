using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarFightPropUpdateNotify : BasePacket
{
    public PacketAvatarFightPropUpdateNotify(AvatarDataInfo avatar, uint fightProp) : base(CmdIds.AvatarFightPropNotify)
    {
        var proto = new AvatarFightPropNotify
        {
            AvatarGuid = avatar.Guid
        };

        proto.FightPropMap[fightProp] = avatar.GetFightProp(fightProp);

        SetData(proto);
    }

    // Constructor for batch prop updates (mirrors Java's Map<Integer, Float> overload)
    public PacketAvatarFightPropUpdateNotify(AvatarDataInfo avatar, Dictionary<uint, float> propUpdateList) : base(CmdIds.AvatarFightPropNotify)
    {
        var proto = new AvatarFightPropNotify
        {
            AvatarGuid = avatar.Guid
        };

        foreach (var kv in propUpdateList)
        {
            proto.FightPropMap[kv.Key] = kv.Value;
        }

        SetData(proto);
    }
}
