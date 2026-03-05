using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.Enums;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityTeam : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityTeam;
    
    public PlayerInstance Player { get; }
    
    public EntityTeam(PlayerInstance player)
    {
        Player = player;
        Owner = player;
        Id = (uint)player.World.GetNextEntityId(EntityIdTypeEnum.Team);
        
        InitAbilities();
    }
    
    public void InitAbilities()
    {
        // TODO: Load abilities from levelElementAbilities
        // Similar to Java implementation:
        // var defaultAbilities = GameData.GetConfigGlobalCombat().GetDefaultAbilities();
        // if (defaultAbilities.GetDefaultTeamAbilities() != null)
        //     foreach (var ability in defaultAbilities.GetDefaultTeamAbilities())
        //     {
        //         var data = GameData.GetAbilityData(ability);
        //         if (data != null) Player.World.Host.AbilityManager.AddAbilityToEntity(this, data);
        //     }
        
        // For now, leave empty or implement when GameData is available
    }
    
    public World.World GetWorld()
    {
        return Player.World;
    }
    
    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.Team;
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