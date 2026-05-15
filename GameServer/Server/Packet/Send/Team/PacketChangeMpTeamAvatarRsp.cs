using NahidaImpact.Database.Team;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketChangeMpTeamAvatarRsp : BasePacket
{
    public PacketChangeMpTeamAvatarRsp(PlayerInstance player, TeamInfo teamInfo) : base(CmdIds.ChangeMpTeamAvatarRsp)
    {
        var proto = new ChangeMpTeamAvatarRsp
        {
            CurAvatarGuid = player.TeamManager.GetCurrentCharacterGuid()
        };

        foreach (var guid in teamInfo.AvatarGuidList)
        {
            proto.AvatarGuidList.Add(guid);
        }

        SetData(proto);
    }
}
