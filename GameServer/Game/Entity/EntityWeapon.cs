using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityWeapon : EntityBaseGadget
{
    public override ProtEntityType EntityType => ProtEntityType.Gadget;

    public int GadgetId { get; }
    public int ItemId { get; set; }
    public ulong ItemGuid { get; set; }

    public EntityWeapon(Scene scene, int gadgetId) : base(scene)
    {
        GadgetId = gadgetId;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Weapon);
    }

    public EntityWeapon(Scene scene, int gadgetId, Position pos, Position rot)
        : base(scene, pos, rot)
    {
        GadgetId = gadgetId;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Weapon);
    }

    public override uint EntityTypeId => (uint)GadgetId;

    public override Dictionary<int, float> GetFightProperties()
    {
        var dict = new Dictionary<int, float>();
        foreach (var fp in FightProperties)
            dict[(int)fp.PropType] = fp.PropValue;
        return dict;
    }

    public override SceneEntityInfo ToProto()
    {
        // Weapon entities are embedded in SceneAvatarInfo / SceneMonsterInfo, never standalone
        return null;
    }
}
