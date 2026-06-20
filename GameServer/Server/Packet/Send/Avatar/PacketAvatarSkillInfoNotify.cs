using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillInfoNotify : BasePacket
{
    public PacketAvatarSkillInfoNotify(AvatarDataInfo avatar)
        : base(CmdIds.AvatarSkillInfoNotify)
    {
        var proto = new AvatarSkillInfoNotify
        {
            Guid = avatar.Guid
        };

        foreach (var kv in avatar.SkillLevelMap)
            proto.SkillMap[kv.Key] = new AvatarSkillInfo { MaxChargeCount = kv.Value };

        SetData(proto);
    }
}
