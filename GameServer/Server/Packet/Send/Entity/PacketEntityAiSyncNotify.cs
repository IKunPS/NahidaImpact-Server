using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Entity;

public class PacketEntityAiSyncNotify : BasePacket
{
    public PacketEntityAiSyncNotify(EntityAiSyncNotify notify) : base(CmdIds.EntityAiSyncNotify)
    {
        var proto = new EntityAiSyncNotify();
        foreach (var monsterId in notify.LocalAvatarAlertedMonsterList)
        {
            proto.InfoList.Add(new AiSyncInfo
            {
                EntityId = monsterId,
                HasPathToTarget = true
            });
        }

        SetData(proto);
    }
}
