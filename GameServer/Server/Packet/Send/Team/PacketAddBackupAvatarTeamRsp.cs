using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketAddBackupAvatarTeamRsp : BasePacket
{
    public PacketAddBackupAvatarTeamRsp(int retcode) : base(CmdIds.AddBackupAvatarTeamRsp)
    {
        var proto = new AddBackupAvatarTeamRsp
        {
            Retcode = retcode
        };

        SetData(proto);
    }

    public PacketAddBackupAvatarTeamRsp() : this(0) // RET_SUCC
    {
    }
}
