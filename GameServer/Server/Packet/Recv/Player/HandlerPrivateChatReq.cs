using NahidaImpact.GameServer.Game.Chat;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PrivateChatReq)]
public class HandlerPrivateChatReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = PrivateChatReq.Parser.ParseFrom(data);
        var player = connection.Player!;

        switch (req.ContentCase)
        {
            case PrivateChatReq.ContentOneofCase.Text:
                ChatSystem.Instance.SendPrivateMessage(player, (int)req.TargetUid, req.Text);
                break;
            case PrivateChatReq.ContentOneofCase.Icon:
                ChatSystem.Instance.SendPrivateMessage(player, (int)req.TargetUid, (int)req.Icon);
                break;
        }

        await Task.CompletedTask;
    }
}