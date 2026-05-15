using NahidaImpact.GameServer.Server.Packet.Send.Ability;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Ability;

[Opcode(CmdIds.EvtBulletMoveNotify)]
public class HandlerEvtBulletMoveNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null)
        {
            connection.Stop();
            return;
        }

        var notify = EvtBulletMoveNotify.Parser.ParseFrom(data);
        connection.Player.Scene?.BroadcastPacketToOthers(
            connection.Player,
            new PacketEvtBulletMoveNotify(notify));

        await Task.CompletedTask;
    }
}
