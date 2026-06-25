using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketGetPlayerSocialDetailRsp : BasePacket
{
    public PacketGetPlayerSocialDetailRsp(PlayerInstance player, uint targetUid = 0, uint param = 0) : base(CmdIds.GetPlayerSocialDetailRsp)
    {
        var proto = new GetPlayerSocialDetailRsp
        {
            DetailData = player.SocialManager.GetSocialDetail(targetUid),
            Param = param,
            Retcode = 0
        };
        SetData(proto);
    }
}
