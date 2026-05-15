using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Map;

public class PacketPersonalSceneJumpRsp : BasePacket
{
    public PacketPersonalSceneJumpRsp(int sceneId, Position pos) : base(CmdIds.PersonalSceneJumpRsp)
    {
        var proto = new PersonalSceneJumpRsp
        {
            DestSceneId = (uint)sceneId,
            Retcode = 0,
            DestPos = new Vector
            {
                X = pos.X,
                Y = pos.Y,
                Z = pos.Z
            }
        };

        SetData(proto);
    }
}
