using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.SetPlayerSignatureReq)]
public class HandlerSetPlayerSignatureReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var req = SetPlayerSignatureReq.Parser.ParseFrom(data);
        player.SocialManager.SetSignature(req.Signature ?? "");

        await player.SendPacket(new PacketSetPlayerSignatureRsp(req.Signature));
    }
}
