using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketProudSkillExtraLevelNotify : BasePacket
{
    public PacketProudSkillExtraLevelNotify(AvatarDataInfo avatar, int talentIndex)
        : base(CmdIds.ProudSkillExtraLevelNotify)
    {
        var proto = new ProudSkillExtraLevelNotify
        {
            AvatarGuid = avatar.Guid,
            TalentIndex = (uint)talentIndex
        };
        SetData(proto);
    }
}
