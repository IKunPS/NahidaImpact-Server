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
    
    public EntityGadget(Scene scene, int gadgetId, Position pos) : this(scene, gadgetId, pos, null, null) {
        
    }
    
    public EntityGadget(Scene scene, int gadgetId, Position pos, Position rot) : this(scene, gadgetId, pos, rot, null) {
        
    }

    public EntityGadget(
        Scene scene, int gadgetId, Position pos, Position rot, int campId, int campType) : this(scene, gadgetId, pos,
        rot, null, campId, campType)
    {
        
    }

    public EntityGadget(
        Scene scene, int gadgetId, Position pos, Position rot, GadgetContent content) : this(scene, gadgetId, pos, rot,
        content, 0, 0)
    {
        
    }

    public EntityGadget(
        Scene scene,
        int gadgetId,
        Position pos,
        Position rot,
        GadgetContent content,
        int campId,
        int campType) : base(scene, pos, rot, campId, campType) {
        {
            GadgetId = gadgetId;
            Owner = scene.GetHost()!;
            Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Gadget);

            Properties = new List<PropValue>
            {
                new() { Type = (uint)PlayerProp.PROP_LEVEL, Ival = 1 }
            };
        }
    }
    
    public override uint getEntityTypeId() => (uint)GadgetId;

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
            Type = (uint)PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = (uint)PlayerProp.PROP_LEVEL, Ival = 1 }
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
