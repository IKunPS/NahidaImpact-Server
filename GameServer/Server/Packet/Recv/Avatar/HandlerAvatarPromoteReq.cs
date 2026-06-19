using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.AvatarPromoteReq)]
public class HandlerAvatarPromoteReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = AvatarPromoteReq.Parser.ParseFrom(data);
        var player = connection.Player;
        if (player == null) return;

        await player.AvatarManager.PromoteAvatar(req.Guid);
    }
}
