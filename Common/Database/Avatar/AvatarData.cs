using NahidaImpact.Data.Excel;
using NahidaImpact.Prop;
using NahidaImpact.Proto;
using SqlSugar;

namespace NahidaImpact.Database.Avatar;

[SugarTable("Avatar")]
public class AvatarData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<AvatarDataInfo> Avatars { get; set; } = [];

    #region Static Access

    public static AvatarData? GetAvatarDataByUid(int uid, bool forceReload = false)
    {
        return DatabaseHelper.GetInstance<AvatarData>(uid, forceReload);
    }

    public static AvatarData GetOrCreateAvatarData(int uid)
    {
        return DatabaseHelper.GetInstanceOrCreateNew<AvatarData>(uid);
    }

    public static void SaveAvatarData(AvatarData data)
    {
        DatabaseHelper.UpdateInstance(data);
    }

    public static AvatarDataInfo? AddAvatar(int uid, AvatarDataInfo avatar)
    {
        var data = GetOrCreateAvatarData(uid);
        if (data.Avatars.Any(a => a.AvatarId == avatar.AvatarId))
            return null;
        data.Avatars.Add(avatar);
        DatabaseHelper.UpdateInstance(data);
        return avatar;
    }

    #endregion
}

public class AvatarDataInfo
{
    // ── Identity ──────────────────────────────────────────────
    public uint AvatarId { get; set; }
    public ulong Guid { get; set; }
    public uint SkillDepotId { get; set; }
    public uint BornTime { get; set; }

    // ── Level & Exp ───────────────────────────────────────────
    public uint Level { get; set; } = 1;
    public uint Exp { get; set; }
    public uint PromoteLevel { get; set; }

    // ── Properties (persisted fight + basic props) ────────────
    public List<PropValue> Properties { get; set; } = [];
    public List<FightPropPair> FightProperties { get; set; } = [];

    // ── Weapon ────────────────────────────────────────────────
    public uint WeaponId { get; set; }
    public ulong WeaponGuid { get; set; } = 2281337;

    // ── Relics ────────────────────────────────────────────────
    public List<uint> ReliquaryList { get; set; } = [];
    public List<ulong> EquipGuidList { get; set; } = [];

    // ── Skills & Talents ──────────────────────────────────────
    public Dictionary<uint, uint> SkillLevelMap { get; set; } = [];
    public List<uint> TalentIdList { get; set; } = [];
    public List<uint> ProudSkillList { get; set; } = [];
    public Dictionary<uint, uint> ProudSkillExtraLevelMap { get; set; } = [];
    public uint CoreProudSkillLevel { get; set; }

    // ── Cosmetics ─────────────────────────────────────────────
    public uint WearingFlycloakId { get; set; }
    public uint CostumeId { get; set; }
    public uint WeaponSkinId { get; set; }
    public uint TraceEffectId { get; set; }

    // ── Fetter ────────────────────────────────────────────────
    public uint FetterLevel { get; set; } = 1;
    public uint FetterExp { get; set; }

    // ── Satiation ─────────────────────────────────────────────
    public uint Satiation { get; set; }
    public uint SatiationPenalty { get; set; }

    // ── NameCard ──────────────────────────────────────────────
    public uint NameCardRewardId { get; set; }
    public uint NameCardId { get; set; }

    // ── Misc persisted ────────────────────────────────────────
    public uint AvatarVoice { get; set; }
    public uint CurPlanIndex { get; set; }
    public string AvatarCosmeticInfoJson { get; set; } = "";

    // ── Trial (persisted) ─────────────────────────────────────
    public uint TrialAvatarId { get; set; }
    public uint GrantReason { get; set; }
    public uint FromParentQuestId { get; set; }

    // ═══════════════════════════════════════════════════════════
    //  Runtime-only fields (not serialized to DB)
    // ═══════════════════════════════════════════════════════════

    [SugarColumn(IsIgnore = true)]
    public AvatarDataExcel? AvatarExcel { get; set; }

    [SugarColumn(IsIgnore = true)]
    public uint OwnerUid { get; set; }

    [SugarColumn(IsIgnore = true)]
    public uint LifeState { get; set; } = 1;

    [SugarColumn(IsIgnore = true)]
    public List<uint> Abilities { get; set; } = [];

    [SugarColumn(IsIgnore = true)]
    public uint AvatarType { get; set; } = 1;

    [SugarColumn(IsIgnore = true)]
    public Dictionary<uint, uint> SkillExtraChargeMap { get; set; } = [];

    // ── Constructors ──────────────────────────────────────────

    public AvatarDataInfo() { }

    public AvatarDataInfo(uint avatarId)
    {
        AvatarId = avatarId;
    }

    // ═══════════════════════════════════════════════════════════
    //  Property helpers (mirrors Java Avatar.setFightProperty etc.)
    // ═══════════════════════════════════════════════════════════

    public void SetProp(uint propType, uint value)
    {
        var prop = Properties.Find(p => p.Type == propType);
        if (prop != null)
        {
            prop.Ival = value;
            return;
        }
        Properties.Add(new PropValue { Type = propType, Ival = value });
    }

    public void SetProp(uint propType, float value)
    {
        var prop = Properties.Find(p => p.Type == propType);
        if (prop != null)
        {
            prop.Fval = value;
            return;
        }
        Properties.Add(new PropValue { Type = propType, Fval = value });
    }

    public float GetFightProp(uint propType)
    {
        var pair = FightProperties.Find(p => p.PropType == propType);
        return pair?.PropValue ?? 0f;
    }

    public void SetFightProp(uint propType, float value)
    {
        var pair = FightProperties.Find(p => p.PropType == propType);
        if (pair != null)
        {
            pair.PropValue = value;
            return;
        }
        FightProperties.Add(new FightPropPair { PropType = propType, PropValue = value });
    }

    // ═══════════════════════════════════════════════════════════
    //  InitDefaultProps (mirrors Java Avatar constructor)
    // ═══════════════════════════════════════════════════════════

    public void InitDefaultProps(AvatarDataExcel excel)
    {
        AvatarExcel = excel;
        Properties.Clear();
        FightProperties.Clear();

        Level = 1;
        Exp = 0;
        PromoteLevel = 0;

        SetProp(PlayerProp.PROP_LEVEL, 1);
        SetProp(PlayerProp.PROP_EXP, 0);
        SetProp(PlayerProp.PROP_BREAK_LEVEL, 0);

        float baseHp = (float)excel.HpBase;
        float baseAttack = (float)excel.AttackBase;
        float baseDefense = (float)excel.DefenseBase;

        SetFightProp(FightProp.FIGHT_PROP_BASE_HP, baseHp);
        SetFightProp(FightProp.FIGHT_PROP_CUR_HP, baseHp);
        SetFightProp(FightProp.FIGHT_PROP_MAX_HP, baseHp);

        SetFightProp(FightProp.FIGHT_PROP_BASE_ATTACK, baseAttack);
        SetFightProp(FightProp.FIGHT_PROP_CUR_ATTACK, baseAttack);

        SetFightProp(FightProp.FIGHT_PROP_BASE_DEFENSE, baseDefense);
        SetFightProp(FightProp.FIGHT_PROP_CUR_DEFENSE, baseDefense);

        SetFightProp(FightProp.FIGHT_PROP_CHARGE_EFFICIENCY, (float)excel.ChargeEfficiency);
        SetFightProp(FightProp.FIGHT_PROP_CRITICAL_HURT, (float)excel.CriticalHurt);
        SetFightProp(FightProp.FIGHT_PROP_CRITICAL, (float)excel.Critical);

        SetFightProp(FightProp.FIGHT_PROP_CUR_FIRE_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_ELEC_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_WATER_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_GRASS_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_WIND_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_ICE_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_CUR_ROCK_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_FIRE_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_ELEC_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_WATER_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_GRASS_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_WIND_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_ICE_ENERGY, 100);
        SetFightProp(FightProp.FIGHT_PROP_MAX_ROCK_ENERGY, 100);
    }

    // ═══════════════════════════════════════════════════════════
    //  RecalcStats (stub — mirrors Java Avatar.recalcStats)
    // ═══════════════════════════════════════════════════════════

    public void RecalcStats()
    {
        // TODO: Re-calculate stats based on current equipment, level, promote level,
        //       weapon, relics, set bonuses, proud skills, and constellations.
        //       Mirrors Java Avatar.recalcStats().
    }

    // ═══════════════════════════════════════════════════════════
    //  ToProto (mirrors Java Avatar.toProto)
    // ═══════════════════════════════════════════════════════════

    public AvatarInfo ToProto()
    {
        var proto = new AvatarInfo
        {
            Guid = Guid,
            AvatarId = AvatarId,
            SkillDepotId = SkillDepotId,
            LifeState = LifeState,
            AvatarType = AvatarType,
            BornTime = BornTime,
            CoreProudSkillLevel = CoreProudSkillLevel,
            WearingFlycloakId = WearingFlycloakId,
            CostumeId = CostumeId,
            WeaponSkinId = WeaponSkinId,
            TraceEffectId = TraceEffectId,
            FetterInfo = new AvatarFetterInfo
            {
                ExpLevel = FetterLevel,
                ExpNumber = FetterExp
            }
        };

        // Skill level map
        foreach (var kv in SkillLevelMap)
            proto.SkillLevelMap[kv.Key] = kv.Value;

        // Talent id list
        foreach (var id in TalentIdList)
            proto.TalentIdList.Add(id);

        // Inherent proud skill list
        foreach (var id in ProudSkillList)
            proto.InherentProudSkillList.Add(id);

        // Proud skill extra level map (from constellations)
        foreach (var kv in ProudSkillExtraLevelMap)
            proto.ProudSkillExtraLevelMap[kv.Key] = kv.Value;

        // Fight prop map
        foreach (var fp in FightProperties)
            proto.FightPropMap[fp.PropType] = fp.PropValue;

        // Skill extra charge map
        foreach (var kv in SkillExtraChargeMap)
            proto.SkillMap[kv.Key] = new AvatarSkillInfo { MaxChargeCount = kv.Value };

        // Equip guid list
        proto.EquipGuidList.Add(WeaponGuid);
        foreach (var guid in EquipGuidList)
            proto.EquipGuidList.Add(guid);

        // Prop map
        foreach (var prop in Properties)
            proto.PropMap[prop.Type] = prop;

        return proto;
    }

    // ═══════════════════════════════════════════════════════════
    //  Trial avatar
    // ═══════════════════════════════════════════════════════════

    public void SetTrialAvatarInfo(uint weaponId, uint trialAvatarId, int reason, int questMainId)
    {
        TrialAvatarId = trialAvatarId;
        WeaponId = weaponId;
        GrantReason = (uint)reason;
        FromParentQuestId = (uint)questMainId;
        AvatarType = 2; // TRIAL
    }

    public void EquipTrialItems()
    {
        // TODO: Equip trial weapons and relics.
        //       Mirrors Java Avatar.equipTrialItems().
    }

    // ═══════════════════════════════════════════════════════════
    //  Misc
    // ═══════════════════════════════════════════════════════════

    public void SetOwnerUid(uint uid)
    {
        OwnerUid = uid;
    }

    public void Save()
    {
        // Individual save handled through AvatarData.SaveAvatarData at the player level.
        // This is a convenience stub for callers that follow the Java pattern.
    }

    public void SendSkillExtraChargeMap()
    {
        // TODO: Send skill extra charge map to client.
        //       Mirrors Java Avatar.sendSkillExtraChargeMap().
    }

    public void BuildEmbryo()
    {
        if (AvatarExcel == null) return;
        // TODO: Build ability embryos from avatar data config.
        //       Mirrors Java AvatarData.buildEmbryo().
    }
}