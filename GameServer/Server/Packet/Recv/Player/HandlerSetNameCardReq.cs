using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.SetNameCardReq)]
public class HandlerSetNameCardReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = SetNameCardReq.Parser.ParseFrom(data);
        var success = player.SocialManager.SetNameCard(req.NameCardId);

        await player.SendPacket(new PacketSetNameCardRsp(success, req.NameCardId, player.Profile.NameCardId));
    }
}
