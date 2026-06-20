using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarSkillChangeNotify : BasePacket
{
    public PacketAvatarSkillChangeNotify(AvatarDataInfo avatar, int skillId, int oldLevel, int newLevel)
        : base(CmdIds.AvatarSkillChangeNotify)
    {
        var proto = new AvatarSkillChangeNotify
        {
            AvatarGuid = avatar.Guid,
            AvatarSkillId = (uint)skillId,
            OldLevel = (uint)oldLevel,
            CurLevel = (uint)newLevel,
            SkillDepotId = avatar.SkillDepotId
        };
        SetData(proto);
    }
}