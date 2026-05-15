using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketWorldPlayerDieNotify : BasePacket
{
    public PacketWorldPlayerDieNotify(uint dieType, int killedBy) : base(CmdIds.WorldPlayerDieNotify)
    {
        var proto = new WorldPlayerDieNotify
        {
            DieType = (PlayerDieType)dieType,
            MonsterId = (uint)killedBy
        };

        SetData(proto);
    }
}
