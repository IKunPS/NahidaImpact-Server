using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Avatar;

public class PacketAvatarDieAnimationEndRsp : BasePacket
{
    public PacketAvatarDieAnimationEndRsp(uint dieGuid, int retcode) : base(CmdIds.AvatarDieAnimationEndRsp)
    {
        var proto = new AvatarDieAnimationEndRsp
        {
            DieGuid = dieGuid,
            Retcode = retcode
        };

        SetData(proto);
    }
}
