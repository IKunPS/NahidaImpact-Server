using System.Collections.Generic;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketDelTeamEntityNotify : BasePacket
{
    public PacketDelTeamEntityNotify(int sceneId, int teamEntityId) : base(CmdIds.DelTeamEntityNotify)
    {
        var proto = new DelTeamEntityNotify
        {
            SceneId = (uint)sceneId
        };
        proto.DelEntityIdList.Add((uint)teamEntityId);

        SetData(proto);
    }

    public PacketDelTeamEntityNotify(int sceneId, List<int> list) : base(CmdIds.DelTeamEntityNotify)
    {
        var proto = new DelTeamEntityNotify
        {
            SceneId = (uint)sceneId
        };
        foreach (var id in list)
            proto.DelEntityIdList.Add((uint)id);

        SetData(proto);
    }
}
