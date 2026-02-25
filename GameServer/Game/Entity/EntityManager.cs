using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Scene;
using NahidaImpact.GameServer.Server.Packet.Send.Scene;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityManager
{
    private readonly List<SceneEntity> _entities;
    private readonly PlayerInstance player;
    
    public EntityManager(PlayerInstance listener)
    {
        _entities = new List<SceneEntity>();
        player = listener;
    }
    
    public async ValueTask AddEntityAsync(SceneEntity entity, VisionType visionType)
    {
        _entities.Add(entity);
        await player.SendPacket(new PacketSceneEntityAppearNotify(entity, visionType));
    }
    
    public void Reset()
    {
        _entities.Clear();
    }
}