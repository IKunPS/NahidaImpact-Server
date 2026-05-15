using NahidaImpact.GameServer.Game.Chat;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PullRecentChatReq)]
public class HandlerPullRecentChatReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        ChatSystem.Instance.HandlePullRecentChatReq(connection.Player!);

        await Task.CompletedTask;
    }
}