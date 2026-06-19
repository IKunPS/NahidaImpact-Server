using NahidaImpact.Enums.Player;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Worlds;

public class TeleportProperties
{
    public int SceneId { get; set; }
    public int DungeonId { get; set; }
    public TeleportType TeleportType { get; set; }
    public EnterReason EnterReason { get; set; }
    public Position? TeleportTo { get; set; }
    public Position? TeleportRot { get; set; }
    public EnterType EnterType { get; set; } = EnterType.Jump;

    public TeleportProperties() { }

    public TeleportProperties(int sceneId, TeleportType teleportType, Position teleportTo)
    {
        SceneId = sceneId;
        TeleportType = teleportType;
        TeleportTo = teleportTo;
    }
}
