using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.GetPlayerSocialDetailReq)]
public class HandlerGetPlayerSocialDetailReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = GetPlayerSocialDetailReq.Parser.ParseFrom(data);

        if (req.Uid > 0 && req.Uid != player.Uid)
        {
            var targetPlayer = PlayerInstance.GetPlayerInstanceByUid(req.Uid);
            if (targetPlayer != null)
            {
                await player.SendPacket(new PacketGetPlayerSocialDetailRsp(targetPlayer, req.Uid, req.Param));
            }
            else
            {
                // Target player not online — send minimal offline profile
                var rsp = new GetPlayerSocialDetailRsp
                {
                    DetailData = new SocialDetail
                    {
                        Uid = req.Uid,
                        OnlineState = FriendOnlineState.FreiendDisconnect
                    },
                    Param = req.Param,
                    Retcode = 0
                };
                var packet = new BasePacket(CmdIds.GetPlayerSocialDetailRsp);
                packet.SetData(rsp);
                await player.SendPacket(packet);
            }
        }
        else
        {
            await player.SendPacket(new PacketGetPlayerSocialDetailRsp(player, 0, req.Param));
        }
    }
}
