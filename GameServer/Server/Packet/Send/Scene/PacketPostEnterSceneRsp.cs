using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Scene;

public class PacketPostEnterSceneRsp : BasePacket
{
    public PacketPostEnterSceneRsp(PlayerInstance player) : base(CmdIds.PostEnterSceneRsp)
    {
        var proto = new PostEnterSceneRsp()
        {
            EnterSceneToken = (player.EnterToken ^ 59003) + 18565
        };
        
        SetData(proto);
    }
}