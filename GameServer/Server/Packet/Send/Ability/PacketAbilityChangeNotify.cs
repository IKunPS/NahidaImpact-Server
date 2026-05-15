using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Game.Entity;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

public class PacketAbilityChangeNotify : BasePacket
{
    public PacketAbilityChangeNotify(EntityAvatar entity) : base(CmdIds.AbilityChangeNotify)
    {
        var notify = new AbilityChangeNotify
        {
            EntityId = entity.Id,
            AbilityControlBlock = entity.GetAbilityControlBlock()
        };

        SetData(notify);
    }
}
