using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityWeapon : EntityBaseGadget
{
    public override ProtEntityType EntityType => ProtEntityType.Gadget;

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

    // hk4e: weapon entities appear as gadgets in the scene via SceneGadgetInfo
    public override SceneEntityInfo ToProto()
    {
        var info = new SceneEntityInfo
        {
            EntityId = Id,
            EntityType = ProtEntityType.Gadget,
            MotionInfo = GetMotionInfo(),
            LifeState = 1,
        };
        info.Gadget = new SceneGadgetInfo
        {
            GadgetId = (uint)GadgetId,
            GadgetType = (uint)(GadgetId / 10000000),
            BornType = GadgetBornType.Gadget,
            AuthorityPeerId = Scene?.World?.HostPeerId ?? 0,
            ConfigId = (uint)ConfigId,
            GroupId = (uint)GroupId,
            OwnerEntityId = Id,
        };
        return info;
    }
}
