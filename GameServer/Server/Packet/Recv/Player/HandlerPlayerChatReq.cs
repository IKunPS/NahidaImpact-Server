using NahidaImpact.GameServer.Game.Chat;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PlayerChatReq)]
public class HandlerPlayerChatReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null) return;
        var req = PlayerChatReq.Parser.ParseFrom(data);
        var player = connection.Player;

        switch (req.ChatInfo.ContentCase)
        {
            case ChatInfo.ContentOneofCase.Text:
                ChatSystem.Instance.SendTeamMessage(player, (int)req.ChannelId, req.ChatInfo.Text);
                break;
            case ChatInfo.ContentOneofCase.Icon:
                ChatSystem.Instance.SendTeamMessage(player, (int)req.ChannelId, (int)req.ChatInfo.Icon);
                break;
        }

        await connection.SendPacket(new PacketPlayerChatRsp());
    }
}