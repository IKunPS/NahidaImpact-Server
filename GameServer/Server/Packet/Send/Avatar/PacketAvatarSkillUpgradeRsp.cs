using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillUpgradeRsp : BasePacket
{
    public PacketAvatarSkillUpgradeRsp(AvatarDataInfo avatar, int skillId, int oldLevel, int newLevel)
        : base(CmdIds.AvatarSkillUpgradeRsp)
    {
        var proto = new AvatarSkillUpgradeRsp
        {
            AvatarGuid = avatar.Guid,
            AvatarSkillId = (uint)skillId,
            OldLevel = (uint)oldLevel,
            CurLevel = (uint)newLevel
        };
        SetData(proto);
    }
}