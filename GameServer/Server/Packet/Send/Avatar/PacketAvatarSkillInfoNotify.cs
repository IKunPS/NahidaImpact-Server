using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillInfoNotify : BasePacket
{
    // Sends skill extra charge counts (hk4e: Avatar::sendSkillExtraChargeMap).
    // SkillExtraChargeMap stores bonus charges per skill from constellations.
    public PacketAvatarSkillInfoNotify(AvatarDataInfo avatar)
        : base(CmdIds.AvatarSkillInfoNotify)
    {
        var proto = new AvatarSkillInfoNotify
        {
            Guid = avatar.Guid
        };

        foreach (var kv in avatar.SkillExtraChargeMap)
            proto.SkillMap[kv.Key] = new AvatarSkillInfo { MaxChargeCount = kv.Value };

        SetData(proto);
    }
}
