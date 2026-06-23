using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Entity.Gadget;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityGadget : EntityBaseGadget
{
    public override ProtEntityType EntityType => ProtEntityType.Gadget;

    public GadgetContent? Content { get; private set; }
    public GadgetDataExcel? GadgetData { get; }

    public EntityGadget(Scene scene, int gadgetId, Position? pos = null, Position? rot = null,
        GadgetContent? content = null, int campId = 0, int campType = 0)
        : base(scene, pos, rot, campId, campType)
    {
        GadgetId = gadgetId;
        GadgetData = GameData.GadgetData.GetValueOrDefault(gadgetId);
        Owner = scene.Host!;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Gadget);
        CreateTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Properties = [new() { Type = PlayerProp.PROP_LEVEL, Ival = 1 }];

        InitFightProperties();
        InitAbilities();

        if (content != null)
            SetContent(content);
    }

    private void InitFightProperties()
    {
        FightProperties = new List<FightPropPair>
        {
            new() { PropType = FightProp.FIGHT_PROP_CUR_HP, PropValue = float.PositiveInfinity },
            new() { PropType = FightProp.FIGHT_PROP_MAX_HP, PropValue = float.PositiveInfinity },
            new() { PropType = FightProp.FIGHT_PROP_BASE_HP, PropValue = float.PositiveInfinity },
        };
    }

    private void InitAbilities()
    {
        var levelEntityConfig = Scene?.SceneData?.LevelEntityConfig;
        if (!string.IsNullOrEmpty(levelEntityConfig))
        {
            var config = GameData.ConfigLevelEntityDataMap.GetValueOrDefault(levelEntityConfig);
            if (config?.Abilities != null)
            {
                foreach (var ga in config.Abilities)
                    AddConfigAbility(ga.AbilityName);
            }
        }
    }

    private void AddConfigAbility(string name)
    {
        var data = GameData.GetAbilityData(name);
        if (data != null)
            Owner?.AbilityManager?.AddAbilityToEntity(this, data);
    }

    public void SetContent(GadgetContent content)
    {
        Content = content;
    }

    // hk4e Gadget state machine — transitions trigger ability events
    public void UpdateState(uint newState)
    {
        if (GadgetState == newState) return;
        var oldState = GadgetState;
        SetState(newState);
        Content?.OnStateChanged(oldState, newState);
    }

    public override uint EntityTypeId => (uint)GadgetId;

    public override Dictionary<int, float> GetFightProperties()
    {
        var dict = new Dictionary<int, float>();
        foreach (var fp in FightProperties)
            dict[(int)fp.PropType] = fp.PropValue;
        return dict;
    }

    public override void OnInteract(PlayerInstance player, object interactReq)
    {
        Content?.OnInteract(player, (GadgetInteractReq)interactReq);
    }

    public override SceneEntityInfo ToProto()
    {
        var info = new SceneEntityInfo
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

        info.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());
        info.PropList.Add(new PropPair
        {
            Type = PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = 1 }
        });

        foreach (var fp in FightProperties)
            info.FightPropList.Add(fp);

        var gadgetInfo = new SceneGadgetInfo
        {
            GadgetId = (uint)GadgetId,
            GadgetType = (uint)(GadgetData?.Type ?? 0),
            GadgetState = GadgetState,
            IsEnableInteract = IsEnableInteract,
            BornType = BornType,
            AuthorityPeerId = Scene?.World?.HostPeerId ?? 0,
            ConfigId = (uint)ConfigId,
            GroupId = (uint)GroupId,
            OwnerEntityId = Id,
            PropOwnerEntityId = (uint)(PropOwner?.Uid ?? 0),
            DraftId = 3000,
            MarkFlag = 0,
        };

        foreach (var uid in InteractUids)
            gadgetInfo.InteractUidList.Add(uid);

        Content?.OnBuildProto(gadgetInfo);
        info.Gadget = gadgetInfo;
        return info;
    }
}
