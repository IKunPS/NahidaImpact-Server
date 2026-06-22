using NahidaImpact.Data;
using NahidaImpact.Data.Binout;
using NahidaImpact.Data.Excel;
using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityMonster : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.Monster;
    
    private readonly Position position;
    private readonly Position rotation;

    public MonsterDataExcel MonsterData { get; }
    public ConfigEntityMonster? ConfigEntityMonster { get; }

    public override uint getEntityTypeId() => MonsterId;

    private uint MonsterId => MonsterData.Id;

    public override Position GetPosition() => position;
    public override Position GetRotation() => rotation;
    
    public int Level { get; }
    public Position BornPos { get; }
    public int PoseId { get; set; }
    public int AiId { get; set; } = -1;
    public int SummonedTag { get; set; }
    public int OwnerEntityId { get; set; }

    public EntityMonster(Scene scene, MonsterDataExcel monsterData, Position pos, Position? rot, int level = 1) : base(scene)
    {
        MonsterData = monsterData;
        Level = level;
        Owner = scene.GetHost()!;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Monster);
        position = new Position(pos);
        rotation = rot != null ? new Position(rot) : new Position();
        BornPos = pos.Clone();

        FightProperties = new List<FightPropPair>
        {
            new() { PropType = FightProp.FIGHT_PROP_CUR_HP, PropValue = 1000f * level },
            new() { PropType = FightProp.FIGHT_PROP_MAX_HP, PropValue = 1000f * level },
            new() { PropType = FightProp.FIGHT_PROP_BASE_HP, PropValue = 1000f * level },
            new() { PropType = FightProp.FIGHT_PROP_CUR_ATTACK, PropValue = 100f * level },
            new() { PropType = FightProp.FIGHT_PROP_BASE_ATTACK, PropValue = 100f * level },
            new() { PropType = FightProp.FIGHT_PROP_CUR_DEFENSE, PropValue = 100f * level },
            new() { PropType = FightProp.FIGHT_PROP_BASE_DEFENSE, PropValue = 100f * level },
            new() { PropType = FightProp.FIGHT_PROP_CUR_SPEED, PropValue = 2f },
            new() { PropType = FightProp.FIGHT_PROP_BASE_SPEED, PropValue = 2f },
            new() { PropType = FightProp.FIGHT_PROP_CRITICAL, PropValue = 0.05f },
            new() { PropType = FightProp.FIGHT_PROP_CRITICAL_HURT, PropValue = 0.5f },
        };

        Properties = new List<PropValue>
        {
            new() { Type = PlayerProp.PROP_LEVEL, Ival = level }
        };

        InitAbilities();
    }

    private void AddConfigAbility(string name)
    {
        var data = GameData.GetAbilityData(name);
        if (data != null)
            Owner?.AbilityManager?.AddAbilityToEntity(this, data);
    }

    public void InitAbilities()
    {
        // 1. Affix abilities (from MonsterData affix list)
        if (MonsterData.Affix is { Count: > 0 })
        {
            // TODO: Load MonsterAffixData and add affix abilities
            // Requires MonsterAffixData loading infrastructure
        }

        // 2. NonHumanoidMoveAbilities (basic movement - all monsters need this)
        var defaultAbilities = GameData.GetConfigGlobalCombat()?.DefaultAbilities;
        if (defaultAbilities?.NonHumanoidMoveAbilities != null)
        {
            foreach (var abilityName in defaultAbilities.NonHumanoidMoveAbilities)
                AddConfigAbility(abilityName);
        }

        // 3. Monster-specific abilities from its own config (not ALL configs!)
        if (ConfigEntityMonster?.Abilities != null)
        {
            foreach (var configAbilityData in ConfigEntityMonster.Abilities)
                AddConfigAbility(configAbilityData.AbilityName);
        }

        // 4. Elite monster ability
        // TODO: Check group monster isElite and add MonterEliteAbilityName
        // Requires scene group loading infrastructure

        // 5. Scene-level monster abilities
        var levelEntityConfig = Scene?.SceneData?.LevelEntityConfig;
        if (!string.IsNullOrEmpty(levelEntityConfig))
        {
            var config = GameData.GetConfigLevelEntityDataMap().GetValueOrDefault(levelEntityConfig);
            if (config?.MonsterAbilities != null)
            {
                foreach (var ma in config.MonsterAbilities)
                    AddConfigAbility(ma.AbilityName);
            }
        }
    }

    public override Dictionary<int, float> GetFightProperties()
    {
        var dict = new Dictionary<int, float>();
        foreach (var fp in FightProperties)
            dict[(int)fp.PropType] = fp.PropValue;
        return dict;
    }

    public override SceneEntityInfo ToProto()
    {
        var aiInfo = new SceneEntityAiInfo
        {
        };

        if (OwnerEntityId != 0)
        {
            aiInfo.ServantInfo = new ServantInfo { MasterEntityId = (uint)OwnerEntityId };
        }

        var authority = new EntityAuthorityInfo
        {
            AbilityInfo = new AbilitySyncStateInfo(),
            RendererChangedInfo = new EntityRendererChangedInfo(),
            AiInfo = aiInfo,
            BornPos = BornPos.ToProto()
        };

        var entityInfo = new SceneEntityInfo
        {
            EntityId = Id,
            EntityType = ProtEntityType.Monster,
            MotionInfo = GetMotionInfo(),
            LifeState = 1,
            EntityClientData = new EntityClientData(),
            EntityAuthorityInfo = authority
        };

        entityInfo.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());
        entityInfo.PropList.Add(new PropPair
        {
            Type = PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = Level }
        });

        foreach (var fp in FightProperties)
            entityInfo.FightPropList.Add(fp);

        var monsterInfo = new SceneMonsterInfo
        {
            MonsterId = MonsterId,
            GroupId = (uint)GroupId,
            ConfigId = (uint)ConfigId,
            BornType = MonsterBornType.MonsterBornDefault,
            AuthorityPeerId = Scene?.World?.GetHostPeerId() ?? 0,
            BlockId = (uint)(Scene?.Id ?? 0),
            PoseId = (uint)PoseId,
            SummonedTag = (uint)SummonedTag,
            OwnerEntityId = (uint)OwnerEntityId,
        };

        if (AiId != -1)
        {
            monsterInfo.AiConfigId = (uint)AiId;
        }

        // Affix list
        foreach (uint affix in MonsterData.Affix)
            monsterInfo.AffixList.Add(affix);

        entityInfo.Monster = monsterInfo;

        return entityInfo;
    }
}
