using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.GetAllUnlockNameCardReq)]
public class HandlerGetAllUnlockNameCardReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null) return;

        var nameCardList = player.NameCardList.Select(id => (uint)id);
        await player.SendPacket(new PacketGetAllUnlockNameCardRsp(nameCardList));
    }
}
