using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketUnlockAvatarTalentRsp : BasePacket
{
    public PacketUnlockAvatarTalentRsp(AvatarDataInfo avatar, int talentId)
        : base(CmdIds.UnlockAvatarTalentRsp)
    {
        var proto = new UnlockAvatarTalentRsp
        {
            AvatarGuid = avatar.Guid,
            TalentId = (uint)talentId
        };
        SetData(proto);
    }
}