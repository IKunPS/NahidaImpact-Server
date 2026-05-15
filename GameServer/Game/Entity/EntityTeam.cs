using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Enums.Entity;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityTeam : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityTeam;

    public PlayerInstance Player { get; }

    public EntityTeam(PlayerInstance player) : base(player.Scene)
    {
        Player = player;
        Owner = player;
        Id = (uint)player.World.GetNextEntityId(EntityIdTypeEnum.Team);

        InitAbilities();
    }

    public void InitAbilities()
    {
        // Load abilities from levelElementAbilities
        var defaultAbilities = Data.GameData.GetConfigGlobalCombat()?.DefaultAbilities;
        if (defaultAbilities?.DefaultTeamAbilities != null)
        {
            foreach (var ability in defaultAbilities.DefaultTeamAbilities)
            {
                var data = Data.GameData.GetAbilityData(ability);
                if (data != null)
                    Player.World?.GetHost()?.AbilityManager?.AddAbilityToEntity(this, data);
            }
        }
    }

    public World GetWorld()
    {
        return Player.World!;
    }

    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.Team;
    }

    public override Dictionary<int, float> GetFightProperties()
    {
        // TODO: Return appropriate fight properties
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

    public override SceneEntityInfo? ToProto()
    {
        // Matches Java — returns null for EntityTeam
        return null;
    }
}
