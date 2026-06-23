using NahidaImpact.Data;
using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Entity;

// hk4e PlayTeamEntity — represents the player's party as an entity in scene
public class EntityTeam : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.Team;

    public EntityTeam(Scene scene) : base(scene)
    {
        Owner = scene.Host!;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Team);
        FightProperties = [];
        Properties = [];
        InitAbilities();
    }

    private void InitAbilities()
    {
        var defaults = GameData.ConfigGlobalCombat?.DefaultAbilities;
        if (defaults?.DefaultTeamAbilities != null)
        {
            foreach (var name in defaults.DefaultTeamAbilities)
            {
                var data = GameData.GetAbilityData(name);
                if (data != null)
                    Owner?.AbilityManager?.AddAbilityToEntity(this, data);
            }
        }

        var levelConfig = Scene?.SceneData?.LevelEntityConfig;
        if (!string.IsNullOrEmpty(levelConfig)
            && GameData.ConfigLevelEntityDataMap.TryGetValue(levelConfig, out var config))
        {
            if (config?.TeamAbilities != null)
            {
                foreach (var a in config.TeamAbilities)
                {
                    var data = GameData.GetAbilityData(a.AbilityName);
                    if (data != null)
                        Owner?.AbilityManager?.AddAbilityToEntity(this, data);
                }
            }
        }
    }

    public override uint EntityTypeId => (uint)EntityIdTypeEnum.Team;
    public override Position Position => Position.Zero;
    public override Position Rotation => Position.Zero;

    public override Dictionary<int, float> GetFightProperties()
    {
        var dict = new Dictionary<int, float>();
        foreach (var fp in FightProperties)
            dict[(int)fp.PropType] = fp.PropValue;
        return dict;
    }

    // hk4e: Team entities use PlayTeamEntityInfo (separate proto), not SceneEntityInfo
    public PlayTeamEntityInfo ToPlayTeamProto()
    {
        return new PlayTeamEntityInfo
        {
            EntityId = Id,
            PlayerUid = (uint)(Owner?.Uid ?? 0),
            AuthorityPeerId = Scene?.World?.HostPeerId ?? 0,
            AbilityInfo = new AbilitySyncStateInfo()
        };
    }

    public override SceneEntityInfo ToProto() => null;
}
