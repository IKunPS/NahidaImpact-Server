using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.Proto;
using System.Collections.Generic;

namespace NahidaImpact.GameServer.Game.Entity;

public abstract class BaseEntity
{
    public abstract ProtEntityType EntityType { get; }
    public uint Id { get; set; }
    public PlayerInstance? Owner { get; set; }
    
    public MotionInfo MotionInfo { get; set; }
    public MotionState MotionState { get; set; }
    public List<PropValue> Properties { get; set; }
    public List<FightPropPair> FightProperties { get; set; }
    
    // Ability system properties
    public List<Ability.Ability> InstancedAbilities { get; set; } = new();
    public Dictionary<int, AbilityModifierController> InstancedModifiers { get; set; } = new();
    public Dictionary<string, float> GlobalAbilityValues { get; set; } = new();

    // Abstract methods for position and rotation (mirroring Java GameEntity)
    public abstract Position GetPosition();
    public abstract Position GetRotation();

    public BaseEntity()
    {
        MotionInfo = new() { Pos = new(), Rot = new(), Speed = new() };
        MotionState = MotionState.MotionNone;
        Properties = new();
        FightProperties = new();
    }
    
    public abstract uint getEntityTypeId();

    public abstract SceneEntityInfo ToProto();
    
    public virtual MotionInfo GetMotionInfo()
    {
        var motionInfo = new MotionInfo
        {
            Pos = GetPosition().ToProto(),
            Rot = GetRotation().ToProto(),
            Speed = new Vector(),
            State = MotionState
        };
        return motionInfo;
    }
    
    public virtual void Move(Position position, Position rotation)
    {
        // Set the position and rotation.
        GetPosition().Set(position);
        GetRotation().Set(rotation);
    }
    
    public virtual void OnCreate()
    {
        // Base implementation - can be overridden by derived classes
    }
    
    public virtual void OnRemoved()
    {
        // Base implementation - can be overridden by derived classes
    }
}