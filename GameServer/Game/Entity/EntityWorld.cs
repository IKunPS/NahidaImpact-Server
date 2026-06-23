using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Enums.Entity;
using NahidaImpact.Proto;

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

    public override uint EntityTypeId => (uint)EntityIdTypeEnum.MpLevel;

    public void InitAbilities()
    {
        var defaultAbilities = Data.GameData.ConfigGlobalCombat?.DefaultAbilities;
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

    public override Position Position => new() { X = 0, Y = 0, Z = 0 };

    public override Position Rotation => new() { X = 0, Y = 0, Z = 0 };

    // hk4e: MP_LEVEL entities use MPLevelEntityInfo (separate proto), not SceneEntityInfo
    public MPLevelEntityInfo ToMpLevelProto()
    {
        return new MPLevelEntityInfo
        {
            EntityId = Id,
            AuthorityPeerId = World.HostPeerId,
            AbilityInfo = new AbilitySyncStateInfo()
        };
    }

    public override SceneEntityInfo ToProto() => null;
}
