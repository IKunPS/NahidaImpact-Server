using NahidaImpact.GameServer.Game.Chat;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PullPrivateChatReq)]
public class HandlerPullPrivateChatReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = PullPrivateChatReq.Parser.ParseFrom(data);

        ChatSystem.Instance.HandlePullPrivateChatReq(connection.Player!, (int)req.TargetUid);

        await Task.CompletedTask;
    }
}