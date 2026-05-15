using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Ability;

public class PacketEvtBulletMoveNotify : BasePacket
{
    public PacketEvtBulletMoveNotify(EvtBulletMoveNotify notify) : base(CmdIds.EvtBulletMoveNotify)
    {
        SetData(notify);
    }
}
