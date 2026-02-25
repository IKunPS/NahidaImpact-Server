using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketPlayerEnterSceneNotify : BasePacket
{
    public PacketPlayerEnterSceneNotify(PlayerInstance player) : base(CmdIds.PlayerEnterSceneNotify)
    {
        player.SceneManager.EnterToken = (uint)Extensions.RandomInt(1000, 99999);
        
        var proto = new PlayerEnterSceneNotify
        {
            SceneBeginTime = (player.SceneManager!.BeginTime ^ 27843) - 16749,
            Type = EnterType.EnterSelf,
            SceneId = (player.SceneManager.SceneId - 49379) ^ 11523,
            SceneTransaction =  player.SceneManager.CreateTransaction(player.SceneManager.SceneId, (uint)player.Uid,  player.SceneManager.BeginTime),
            Pos = new()
            {
                X = 2191.16357421875f,
                Y = 214.65115356445312f,
                Z = -1120.633056640625f
            },
            TargetUid = (uint)(player.Uid - 30259) ^ 4145,
            EnterSceneToken = (player.SceneManager.EnterToken ^ 57361) - 22665
        };
        
        SetData(proto);
    }
}