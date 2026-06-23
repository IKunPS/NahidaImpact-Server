using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Event.Player;

public class PlayerFlyEvent
{
    public PlayerInstance Player { get; }
    public EntityAvatar Avatar { get; }
    public bool IsStart { get; }      // true = started flying, false = stopped
    public float FlyTime { get; }     // seconds spent flying (only valid when IsStart=false)
    public double FlyDistance { get; } // distance covered during flight (only valid when IsStart=false)
    public Position StartPos { get; }

    public PlayerFlyEvent(PlayerInstance player, EntityAvatar avatar, bool isStart,
        float flyTime = 0, double flyDistance = 0, Position? startPos = null)
    {
        Player = player;
        Avatar = avatar;
        IsStart = isStart;
        FlyTime = flyTime;
        FlyDistance = flyDistance;
        StartPos = startPos ?? new Position();
    }

    public void Call()
    {
        // TODO: PlayerWatcherComp::setIsFlyInLastTimeInterval, tryUpdateFlyRecord
    }
}
