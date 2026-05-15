using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Team;

public class PacketDelBackupAvatarTeamRsp : BasePacket
{
    public PacketDelBackupAvatarTeamRsp(int retcode, int id) : base(CmdIds.DelBackupAvatarTeamRsp)
    {
        var proto = new DelBackupAvatarTeamRsp
        {
            Retcode = retcode,
            BackupAvatarTeamId = (uint)id
        };

        SetData(proto);
    }

    public PacketDelBackupAvatarTeamRsp(int id) : this(0, id) // RET_SUCC
    {
    }
}
