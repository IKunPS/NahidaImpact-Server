using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.SetPlayerHeadImageReq)]
public class HandlerSetPlayerHeadImageReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = SetPlayerHeadImageReq.Parser.ParseFrom(data);
        player.SocialManager.SetHeadImage(req.ProfilePictureId);

        await player.SendPacket(new PacketSetPlayerHeadImageRsp(player.Profile.HeadImage));
    }
}
