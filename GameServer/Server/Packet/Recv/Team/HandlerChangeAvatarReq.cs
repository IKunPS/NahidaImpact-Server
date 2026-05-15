using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.ChangeAvatarReq)]
public class HandlerChangeAvatarReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = ChangeAvatarReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.ChangeAvatar(req.Guid);

        await Task.CompletedTask;
    }
}
