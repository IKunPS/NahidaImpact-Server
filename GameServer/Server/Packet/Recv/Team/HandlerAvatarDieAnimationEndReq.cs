using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Team;

[Opcode(CmdIds.AvatarDieAnimationEndReq)]
public class HandlerAvatarDieAnimationEndReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = AvatarDieAnimationEndReq.Parser.ParseFrom(data);

        connection.Player?.TeamManager.OnAvatarDie(req.DieGuid);

        await Task.CompletedTask;
    }
}
