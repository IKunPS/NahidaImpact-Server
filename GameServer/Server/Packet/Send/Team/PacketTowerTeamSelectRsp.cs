using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketTowerTeamSelectRsp : BasePacket
{
    public PacketTowerTeamSelectRsp() : base(CmdIds.TowerTeamSelectRsp)
    {
        SetData(new TowerTeamSelectRsp());
    }
}
