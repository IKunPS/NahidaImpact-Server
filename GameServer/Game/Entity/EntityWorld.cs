    using NahidaImpact.Common.Enums;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.Proto;
using System.Collections.Generic;
using NahidaImpact.Enums;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityWorld : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityMpLevel;
    
    public World.World World { get; }
    
    public EntityWorld(World.World world)
    {
        World = world;
        Owner = world.Host;
        Id = (uint)world.GetNextEntityId(EntityIdTypeEnum.MpLevel);
        
        InitAbilities();
    }
    
    public Scene GetScene()
    {
        return World.Host?.Scene;
    }
    
    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.MpLevel;
    }
    
    public void InitAbilities()
    {
        // TODO: Load abilities from levelElementAbilities
        // Similar to Java implementation:
        // foreach (var ability in GameData.GetConfigGlobalCombat().GetDefaultAbilities().GetDefaultMPLevelAbilities())
        // {
        //     var data = GameData.GetAbilityData(ability);
        //     if (data != null) World.Host.AbilityManager.AddAbilityToEntity(this, data);
        // }
        
        // For now, leave empty or implement when GameData is available
    }
    
    public override Position GetPosition()
    {
        // TODO: Return appropriate position
        return new Position { X = 0, Y = 0, Z = 0 };
    }
    
    public override Position GetRotation()
    {
        // TODO: Return appropriate rotation
        return new Position { X = 0, Y = 0, Z = 0 };
    }
    
    public override SceneEntityInfo ToProto()
    {
        // Similar to Java implementation (returns null for now)
        // TODO: Implement proper conversion
        return null;
    }
}