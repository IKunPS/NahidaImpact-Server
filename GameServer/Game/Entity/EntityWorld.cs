using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Enums.Entity;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityWorld : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.MpLevel;
    
    public World World { get; }
    
    public EntityWorld(World world) : base(world.Host.Scene)
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
        // Load abilities from default MP level abilities
        var defaultAbilities = Data.GameData.GetConfigGlobalCombat()?.DefaultAbilities;
        if (defaultAbilities?.DefaultMpLevelAbilities != null)
        {
            foreach (var ability in defaultAbilities.DefaultMpLevelAbilities)
            {
                var data = Data.GameData.GetAbilityData(ability);
                if (data != null)
                    World.Host.AbilityManager?.AddAbilityToEntity(this, data);
            }
        }
    }

    public override Dictionary<int, float> GetFightProperties()
    {
        return new Dictionary<int, float>();
    }

    public override Position GetPosition()
    {
        return new Position { X = 0, Y = 0, Z = 0 };
    }

    public override Position GetRotation()
    {
        return new Position { X = 0, Y = 0, Z = 0 };
    }

    public override SceneEntityInfo ToProto()
    {
        return null;
    }
}