using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet.Send.Team;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Tower;

[Opcode(CmdIds.TowerTeamSelectReq)]
public class HandlerTowerTeamSelectReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = TowerTeamSelectReq.Parser.ParseFrom(data);

        // Parse floor data — tower team lists would be extracted from the request
        // Java reads team list from TowerTeamSelectReq
        // For now, mirror basic Java behavior
        var player = connection.Player;
        if (player == null) return;

        // TODO: Full tower system implementation
        // TowerManager.teamSelect(req.getFloorId(), req.getTowerTeamListList());

        await player.SendPacket(new PacketTowerTeamSelectRsp());

        await Task.CompletedTask;
    }
}
