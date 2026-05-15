using NahidaImpact.GameServer.Server.Packet.Send.Entity;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Entity;

[Opcode(CmdIds.EntityAiSyncNotify)]
public class HandlerEntityAiSyncNotify : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var notify = EntityAiSyncNotify.Parser.ParseFrom(data);
        if (notify.LocalAvatarAlertedMonsterList.Count > 0)
        {
            connection.Player?.Scene?.BroadcastPacket(new PacketEntityAiSyncNotify(notify));
        }
    }
}