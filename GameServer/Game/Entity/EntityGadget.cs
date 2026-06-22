using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Entity.Gadget;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityGadget : EntityBaseGadget
{
    public override ProtEntityType EntityType => ProtEntityType.Gadget;

    public int GadgetId { get; }
    public int GadgetState { get; set; }

    public EntityGadget(Scene scene, int gadgetId, Position? pos = null, Position? rot = null, GadgetContent? content = null, int campId = 0, int campType = 0) : base(scene, pos, rot, campId, campType)
    {
        _ = content; // reserved for future gadget content wiring
        GadgetId = gadgetId;
        Owner = scene.Host!;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Gadget);

        Properties = new List<PropValue>
        {
            new() { Type = PlayerProp.PROP_LEVEL, Ival = 1 }
        };
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
        var entityInfo = new SceneEntityInfo
        {
            EntityId = Id,
            EntityType = ProtEntityType.Gadget,
            MotionInfo = GetMotionInfo(),
            LifeState = 1,
            EntityClientData = new EntityClientData(),
            EntityAuthorityInfo = new EntityAuthorityInfo
            {
                AbilityInfo = new AbilitySyncStateInfo(),
                RendererChangedInfo = new EntityRendererChangedInfo(),
                AiInfo = new SceneEntityAiInfo()
            }
        };

        entityInfo.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());
        entityInfo.PropList.Add(new PropPair
        {
            Type = PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = 1 }
        });

        entityInfo.Gadget = new SceneGadgetInfo
        {
            GadgetId = (uint)GadgetId,
            GadgetState = (uint)GadgetState,
            IsEnableInteract = true
        };

        return entityInfo;
    }
}
