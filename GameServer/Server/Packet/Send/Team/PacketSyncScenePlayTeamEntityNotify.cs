using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketSyncScenePlayTeamEntityNotify : BasePacket
{
    public PacketSyncScenePlayTeamEntityNotify(PlayerInstance player) : base(CmdIds.SyncScenePlayTeamEntityNotify)
    {
        var proto = new SyncScenePlayTeamEntityNotify
        {
            
        };

        SetData(proto);
    }
}
