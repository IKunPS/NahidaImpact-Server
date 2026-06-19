using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Server.Packet.Send.Time;

public class PacketSceneTimeNotify : BasePacket
{
    public PacketSceneTimeNotify(PlayerInstance player) : base(CmdIds.SceneTimeNotify)
    {
        var proto = new SceneTimeNotify
        {
            SceneId = (uint)player.Scene.Id,
            SceneTime = (ulong)player.Scene.SceneTime,
            IsPaused = player.Scene.IsPaused
        };

        SetData(proto);
    }
}