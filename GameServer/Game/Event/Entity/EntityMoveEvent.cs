using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Event.Entity;

public class EntityMoveEvent
{
    public BaseEntity Entity { get; }
    public Position Position { get; }
    public Position Rotation { get; }
    public MotionState MotionState { get; }

    public bool IsCancelled { get; private set; }

    public EntityMoveEvent(BaseEntity entity, Position position, Position rotation, MotionState motionState)
    {
        Entity = entity;
        Position = position;
        Rotation = rotation;
        MotionState = motionState;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    public bool Call()
    {
        return !IsCancelled;
    }
}