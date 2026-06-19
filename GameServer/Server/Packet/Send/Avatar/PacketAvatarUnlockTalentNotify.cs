using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarUnlockTalentNotify : BasePacket
{
    public PacketAvatarUnlockTalentNotify(AvatarDataInfo avatar, int talentId)
        : base(CmdIds.AvatarUnlockTalentNotify)
    {
        var proto = new AvatarUnlockTalentNotify
        {
            AvatarGuid = avatar.Guid,
            TalentId = (uint)talentId,
            SkillDepotId = avatar.SkillDepotId
        };
        SetData(proto);
    }
}