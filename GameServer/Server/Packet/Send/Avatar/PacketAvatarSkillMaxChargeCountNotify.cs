using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillMaxChargeCountNotify : BasePacket
{
    public PacketAvatarSkillMaxChargeCountNotify(AvatarDataInfo avatar, uint skillId, uint maxChargeCount)
        : base(CmdIds.AvatarSkillMaxChargeCountNotify)
    {
        var proto = new AvatarSkillMaxChargeCountNotify
        {
            AvatarGuid = avatar.Guid,
            SkillId = skillId,
            MaxChargeCount = maxChargeCount
        };
        SetData(proto);
    }
}
