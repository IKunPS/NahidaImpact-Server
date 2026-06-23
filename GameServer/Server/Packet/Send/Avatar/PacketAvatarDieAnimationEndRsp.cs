using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarDieAnimationEndRsp : BasePacket
{
    public PacketAvatarDieAnimationEndRsp(ulong dieGuid, int retcode = 0) : base(CmdIds.AvatarDieAnimationEndRsp)
    {
        var proto = new AvatarDieAnimationEndRsp
        {
            DieGuid = dieGuid,
            Retcode = retcode
        };

        SetData(proto);
    }
}
