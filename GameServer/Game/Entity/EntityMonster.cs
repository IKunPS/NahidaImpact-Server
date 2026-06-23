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

    private readonly Position _position;
    private readonly Position _rotation;

    public MonsterDataExcel MonsterData { get; }

    // hk4e Monster core fields
    public uint MonsterId => MonsterData.Id;
    public uint DropId { get; set; }
    public bool IsBanTextDrop { get; set; }
    public bool IsBanAllDrop { get; set; }
    public bool IsElite { get; set; }
    public uint SummonedTag { get; set; }
    public MonsterBornType BornType { get; set; } = MonsterBornType.MonsterBornDefault;
    public uint TitleId { get; set; }
    public uint SpecialNameId { get; set; }
    public uint PoseId { get; set; }
    public uint MonsterPoolId { get; set; }
    public uint KillScore { get; set; }
    public int AiId { get; set; } = -1;
    public uint AiConfigId { get; set; }
    public uint LevelRouteId { get; set; }
    public uint InitPoseId { get; set; }
    public bool IsLightConfig { get; set; }
    public uint MonsterMainTypeId { get; set; }
    public float InitialHpPercentage { get; set; }
    public float DroppedHpPercent { get; set; } = 100f;
    public uint GuestBanDrop { get; set; }
    public uint AttackTargetId { get; set; }
    public uint JsonClimateType { get; set; }
    public uint JsonClimateAreaId { get; set; }
    public uint LuaClimateAreaId { get; set; }
    public uint LastDragBackTime { get; set; }
    public int OwnerEntityId { get; set; }

    public Position BornPos { get; }
    public int Level { get; }

    // hk4e Monster containers
    public HashSet<uint> AffixSet { get; } = [];
    public Dictionary<uint, SummonInfo> SummonMap { get; } = [];
    public HashSet<uint> AlertPlayers { get; } = [];
    public HashSet<uint> AlertPartners { get; } = [];
    public string DropTag { get; set; } = "";

    public ConfigEntityMonster? ConfigEntityMonster { get; }

    public override Position Position => _position;
    public override Position Rotation => _rotation;
    public override uint EntityTypeId => MonsterId;

    public EntityMonster(Scene scene, MonsterDataExcel monsterData, Position pos, Position? rot = null, int level = 1)
        : base(scene)
    {
        MonsterData = monsterData;
        Level = level;
        Owner = scene.Host!;
        Id = (uint)scene.World.GetNextEntityId(EntityIdTypeEnum.Monster);
        _position = new Position(pos);
        _rotation = rot != null ? new Position(rot) : new Position();
        BornPos = pos.Clone();

        MonsterMainTypeId = GetMonsterMainTypeId();
        IsElite = MonsterData.IsElite;

        var hp = MonsterData.GetScaledProp(level, MonsterGrowCurveType.Hp);
        var atk = MonsterData.GetScaledProp(level, MonsterGrowCurveType.Attack);
        var def = MonsterData.GetScaledProp(level, MonsterGrowCurveType.Defense);
        var hpRatio = MonsterData.GetScaledProp(level, MonsterGrowCurveType.HpRatio);
        var atkRatio = MonsterData.GetScaledProp(level, MonsterGrowCurveType.AttackRatio);
        var defRatio = MonsterData.GetScaledProp(level, MonsterGrowCurveType.DefenseRatio);
        var speed = MonsterData.GetScaledProp(level, MonsterGrowCurveType.Speed);

        hp += hp * hpRatio;
        atk += atk * atkRatio;
        def += def * defRatio;

        FightProperties = new List<FightPropPair>
        {
            new() { PropType = FightProp.FIGHT_PROP_CUR_HP, PropValue = hp },
            new() { PropType = FightProp.FIGHT_PROP_MAX_HP, PropValue = hp },
            new() { PropType = FightProp.FIGHT_PROP_BASE_HP, PropValue = MonsterData.HpBase },
            new() { PropType = FightProp.FIGHT_PROP_CUR_ATTACK, PropValue = atk },
            new() { PropType = FightProp.FIGHT_PROP_BASE_ATTACK, PropValue = MonsterData.AttackBase },
            new() { PropType = FightProp.FIGHT_PROP_CUR_DEFENSE, PropValue = def },
            new() { PropType = FightProp.FIGHT_PROP_BASE_DEFENSE, PropValue = MonsterData.DefenseBase },
            new() { PropType = FightProp.FIGHT_PROP_CUR_SPEED, PropValue = speed > 0 ? speed : 2f },
            new() { PropType = FightProp.FIGHT_PROP_BASE_SPEED, PropValue = speed > 0 ? speed : 2f },
            new() { PropType = FightProp.FIGHT_PROP_CRITICAL, PropValue = MonsterData.Critical },
            new() { PropType = FightProp.FIGHT_PROP_CRITICAL_HURT, PropValue = MonsterData.CriticalHurt },
            new() { PropType = FightProp.FIGHT_PROP_ELEMENT_MASTERY, PropValue = MonsterData.ElementMastery },
            // Elemental resistances
            new() { PropType = FightProp.FIGHT_PROP_FIRE_SUB_HURT, PropValue = MonsterData.FireSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_WATER_SUB_HURT, PropValue = MonsterData.WaterSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_GRASS_SUB_HURT, PropValue = MonsterData.GrassSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_ELEC_SUB_HURT, PropValue = MonsterData.ElecSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_ICE_SUB_HURT, PropValue = MonsterData.IceSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_WIND_SUB_HURT, PropValue = MonsterData.WindSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_ROCK_SUB_HURT, PropValue = MonsterData.RockSubHurt },
            new() { PropType = FightProp.FIGHT_PROP_PHYSICAL_SUB_HURT, PropValue = MonsterData.PhysicalSubHurt },
        };

        Properties = new List<PropValue>
        {
            new() { Type = PlayerProp.PROP_LEVEL, Ival = level }
        };

        // Populate affix set from monster data
        foreach (var affix in MonsterData.Affix)
            if (affix > 0) AffixSet.Add((uint)affix);

        InitAbilities();
    }

    // hk4e Monster::getMonsterMainTypeId — extract main type from id prefix
    private uint GetMonsterMainTypeId()
    {
        var id = MonsterId;
        if (id >= 30000000) return (id / 1000) % 10;
        if (id >= 20000000) return (id / 100) % 10;
        return (id / 100000) % 10;
    }

    private void AddConfigAbility(string name)
    {
        var data = GameData.GetAbilityData(name);
        if (data != null)
            Owner?.AbilityManager?.AddAbilityToEntity(this, data);
    }

    private void InitAbilities()
    {
        var defaultAbilities = GameData.ConfigGlobalCombat?.DefaultAbilities;

        // Affix abilities — MonsterAffixExcelConfigData maps affix IDs to ability names
        if (MonsterData.Affix is { Count: > 0 })
        {
            foreach (var affixId in MonsterData.Affix)
            {
                // affix abilities are often named by convention: "MonsterAffix_<id>"
                var abilityName = $"MonsterAffix_{affixId}";
                AddConfigAbility(abilityName);
            }
        }

        // Non-humanoid move abilities
        if (defaultAbilities?.NonHumanoidMoveAbilities != null)
        {
            foreach (var name in defaultAbilities.NonHumanoidMoveAbilities)
                AddConfigAbility(name);
        }

        // Monster-specific abilities
        if (ConfigEntityMonster?.Abilities != null)
        {
            foreach (var a in ConfigEntityMonster.Abilities)
                AddConfigAbility(a.AbilityName);
        }

        // Elite monster ability
        if (IsElite && !string.IsNullOrEmpty(defaultAbilities?.MonterEliteAbilityName))
            AddConfigAbility(defaultAbilities.MonterEliteAbilityName);

        // Level entity config abilities
        var levelEntityConfig = Scene?.SceneData?.LevelEntityConfig;
        if (!string.IsNullOrEmpty(levelEntityConfig))
        {
            var config = GameData.ConfigLevelEntityDataMap.GetValueOrDefault(levelEntityConfig);
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
        var aiInfo = new SceneEntityAiInfo { };
        if (OwnerEntityId != 0)
            aiInfo.ServantInfo = new ServantInfo { MasterEntityId = (uint)OwnerEntityId };

        var authority = new EntityAuthorityInfo
        {
            AbilityInfo = new AbilitySyncStateInfo(),
            RendererChangedInfo = new EntityRendererChangedInfo(),
            AiInfo = aiInfo,
            BornPos = BornPos.ToProto()
        };

        var info = new SceneEntityInfo
        {
            EntityId = Id,
            EntityType = ProtEntityType.Monster,
            MotionInfo = GetMotionInfo(),
            LifeState = 1,
            EntityClientData = new EntityClientData(),
            EntityAuthorityInfo = authority
        };

        info.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());
        info.PropList.Add(new PropPair
        {
            Type = PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = Level }
        });

        foreach (var fp in FightProperties)
            info.FightPropList.Add(fp);

        var monsterInfo = new SceneMonsterInfo
        {
            MonsterId = MonsterId,
            GroupId = (uint)GroupId,
            ConfigId = (uint)ConfigId,
            BornType = BornType,
            AuthorityPeerId = Scene?.World?.HostPeerId ?? 0,
            BlockId = (uint)(Scene?.Id ?? 0),
            PoseId = (uint)PoseId,
            SummonedTag = SummonedTag,
            OwnerEntityId = (uint)OwnerEntityId,
            TitleId = TitleId,
            SpecialNameId = SpecialNameId,
        };

        if (AiId != -1)
            monsterInfo.AiConfigId = (uint)AiId;

        foreach (var affix in AffixSet)
            monsterInfo.AffixList.Add(affix);

        info.Monster = monsterInfo;
        return info;
    }
}

// hk4e SummonInfo — tracks summoned monsters per tag
public class SummonInfo
{
    public uint MaxCount { get; set; }
    public HashSet<EntityMonster> Monsters { get; } = [];
}
