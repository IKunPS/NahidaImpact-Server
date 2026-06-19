using NahidaImpact.Database.Avatar;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarUpgradeRsp : BasePacket
{
    public PacketAvatarUpgradeRsp(AvatarDataInfo avatar, int oldLevel, int curLevel)
        : base(CmdIds.AvatarUpgradeRsp)
    {
        SetData(new AvatarUpgradeRsp
        {
            AvatarGuid = avatar.Guid,
            OldLevel = (uint)oldLevel,
            CurLevel = (uint)curLevel,
            Retcode = 0
        });
    }
}
