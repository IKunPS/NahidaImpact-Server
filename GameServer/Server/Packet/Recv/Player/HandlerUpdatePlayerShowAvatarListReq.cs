using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.UpdatePlayerShowAvatarListReq)]
public class HandlerUpdatePlayerShowAvatarListReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = UpdatePlayerShowAvatarListReq.Parser.ParseFrom(data);
        player.SocialManager.UpdateShowAvatarList(req.ShowAvatarIdList, req.IsShowAvatar);

        await player.SendPacket(new PacketUpdatePlayerShowAvatarListRsp(req.ShowAvatarIdList, req.IsShowAvatar));
    }
}
