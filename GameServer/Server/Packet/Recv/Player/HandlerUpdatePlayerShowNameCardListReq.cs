using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.UpdatePlayerShowNameCardListReq)]
public class HandlerUpdatePlayerShowNameCardListReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = UpdatePlayerShowNameCardListReq.Parser.ParseFrom(data);
        player.SocialManager.UpdateShowNameCardList(req.ShowNameCardIdList);

        await player.SendPacket(new PacketUpdatePlayerShowNameCardListRsp(req.ShowNameCardIdList));
    }
}
