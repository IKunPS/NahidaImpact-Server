using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Avatar;

[Opcode(CmdIds.AvatarUpgradeReq)]
public class HandlerAvatarUpgradeReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = AvatarUpgradeReq.Parser.ParseFrom(data);
        var player = connection.Player;
        if (player == null) return;

        await player.AvatarManager.UpgradeAvatar(req.AvatarGuid, req.ItemParamList.ToList());
    }
}
