using System.Collections.Generic;
using System.Reflection;

namespace NahidaImpact.Prop;

public static class FightProp
{
    /// <summary>Field name (e.g. "FIGHT_PROP_BASE_HP") → uint ID lookup.</summary>
    public static readonly Dictionary<string, uint> NameMap;

    /// <summary>Short name (e.g. "hp", "atk%", "cdmg") → uint ID lookup.</summary>
    public static readonly Dictionary<string, uint> ShortNameMap;

    /// <summary>Reverse lookup: uint ID → field name.</summary>
    public static readonly Dictionary<uint, string> IdMap;

    /// <summary>Flat (non-percentage) prop IDs. Used by IsPercentage().</summary>
    public static readonly HashSet<uint> FlatPropIds;

    /// <summary>Prop IDs that appear as relic main/affix stats.</summary>
    public static readonly int[] StatPropIds;

    /// <summary>Max energy prop IDs for all elements + special.</summary>
    public static readonly uint[] EnergyProps;

    /// <summary>Compound stat definitions: result = base * (1 + percent) + flat.</summary>
    public static readonly Dictionary<uint, CompoundProperty> CompoundProperties;

    /// <summary>Describes how a compound stat is derived from base/percent/flat components.</summary>
    public readonly record struct CompoundProperty(uint Result, uint Base, uint Percent, uint Flat);

    /// <summary>Returns true if the prop is percentage-based (not flat).</summary>
    public static bool IsPercentage(uint propId) => !FlatPropIds.Contains(propId);

    /// <summary>Looks up a prop ID by its short name, falling back to FIGHT_PROP_NONE.</summary>
    public static uint GetPropByShortName(string name) =>
        ShortNameMap.TryGetValue(name, out var id) ? id : FIGHT_PROP_NONE;

    /// <summary>Looks up a prop's field name by its ID.</summary>
    public static string GetPropName(uint propId) =>
        IdMap.TryGetValue(propId, out var name) ? name : nameof(FIGHT_PROP_NONE);

    static FightProp()
    {
        // Build NameMap + IdMap via reflection over all const uint fields
        var nameMap = new Dictionary<string, uint>();
        var idMap = new Dictionary<uint, string>();
        foreach (var field in typeof(FightProp).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(uint) && field.GetValue(null) is uint value)
            {
                nameMap[field.Name] = value;
                idMap[value] = field.Name;
            }
        }
        NameMap = nameMap;
        IdMap = idMap;

        ShortNameMap = new Dictionary<string, uint>
        {
            // Normal relic stats
            ["hp"] = FIGHT_PROP_HP,
            ["atk"] = FIGHT_PROP_ATTACK,
            ["def"] = FIGHT_PROP_DEFENSE,
            ["hp%"] = FIGHT_PROP_HP_PERCENT,
            ["atk%"] = FIGHT_PROP_ATTACK_PERCENT,
            ["def%"] = FIGHT_PROP_DEFENSE_PERCENT,
            ["em"] = FIGHT_PROP_ELEMENT_MASTERY,
            ["er"] = FIGHT_PROP_CHARGE_EFFICIENCY,
            ["hb"] = FIGHT_PROP_HEAL_ADD,
            ["heal"] = FIGHT_PROP_HEAL_ADD,
            ["cd"] = FIGHT_PROP_CRITICAL_HURT,
            ["cdmg"] = FIGHT_PROP_CRITICAL_HURT,
            ["cr"] = FIGHT_PROP_CRITICAL,
            ["crate"] = FIGHT_PROP_CRITICAL,
            ["phys%"] = FIGHT_PROP_PHYSICAL_ADD_HURT,
            ["dendro%"] = FIGHT_PROP_GRASS_ADD_HURT,
            ["geo%"] = FIGHT_PROP_ROCK_ADD_HURT,
            ["anemo%"] = FIGHT_PROP_WIND_ADD_HURT,
            ["hydro%"] = FIGHT_PROP_WATER_ADD_HURT,
            ["cryo%"] = FIGHT_PROP_ICE_ADD_HURT,
            ["electro%"] = FIGHT_PROP_ELEC_ADD_HURT,
            ["pyro%"] = FIGHT_PROP_FIRE_ADD_HURT,
            // Other stats
            ["maxhp"] = FIGHT_PROP_MAX_HP,
            ["debt"] = FIGHT_PROP_CUR_HP_DEBTS,
            ["paiddebt"] = FIGHT_PROP_CUR_HP_PAID_DEBTS,
            ["specialenergy"] = FIGHT_PROP_CUR_SPECIAL_ENERGY,
            ["startspecial"] = FIGHT_PROP_START_SPECIAL_ENERGY,
            ["dmg"] = FIGHT_PROP_ADD_HURT,
            ["cdr"] = FIGHT_PROP_SKILL_CD_MINUS_RATIO,
            ["heali"] = FIGHT_PROP_HEALED_ADD,
            ["shield"] = FIGHT_PROP_SHIELD_COST_MINUS_RATIO,
            ["defi"] = FIGHT_PROP_DEFENCE_IGNORE_RATIO,
            ["resall"] = FIGHT_PROP_SUB_HURT,
            ["resanemo"] = FIGHT_PROP_WIND_SUB_HURT,
            ["rescryo"] = FIGHT_PROP_ICE_SUB_HURT,
            ["resdendro"] = FIGHT_PROP_GRASS_SUB_HURT,
            ["reselectro"] = FIGHT_PROP_ELEC_SUB_HURT,
            ["resgeo"] = FIGHT_PROP_ROCK_SUB_HURT,
            ["reshydro"] = FIGHT_PROP_WATER_SUB_HURT,
            ["respyro"] = FIGHT_PROP_FIRE_SUB_HURT,
            ["resphys"] = FIGHT_PROP_PHYSICAL_SUB_HURT,
        };

        FlatPropIds = new HashSet<uint>
        {
            FIGHT_PROP_BASE_HP,
            FIGHT_PROP_HP,
            FIGHT_PROP_BASE_ATTACK,
            FIGHT_PROP_ATTACK,
            FIGHT_PROP_BASE_DEFENSE,
            FIGHT_PROP_DEFENSE,
            FIGHT_PROP_HEALED_ADD,
            FIGHT_PROP_CUR_FIRE_ENERGY,
            FIGHT_PROP_CUR_ELEC_ENERGY,
            FIGHT_PROP_CUR_WATER_ENERGY,
            FIGHT_PROP_CUR_GRASS_ENERGY,
            FIGHT_PROP_CUR_WIND_ENERGY,
            FIGHT_PROP_CUR_ICE_ENERGY,
            FIGHT_PROP_CUR_ROCK_ENERGY,
            FIGHT_PROP_CUR_SPECIAL_ENERGY,
            FIGHT_PROP_CUR_HP,
            FIGHT_PROP_MAX_HP,
            FIGHT_PROP_CUR_ATTACK,
            FIGHT_PROP_CUR_HP_DEBTS,
            FIGHT_PROP_START_SPECIAL_ENERGY,
            FIGHT_PROP_CUR_HP_PAID_DEBTS,
            FIGHT_PROP_CUR_DEFENSE,
        };

        StatPropIds = new int[]
        {
            1, 4, 7, 20, 21, 22, 23, 26, 27, 28, 29, 30,
            40, 41, 42, 43, 44, 45, 46,
            50, 51, 52, 53, 54, 55, 56,
            2000, 2001, 2002, 2003, 1010
        };

        EnergyProps = new uint[]
        {
            FIGHT_PROP_MAX_FIRE_ENERGY,
            FIGHT_PROP_MAX_ELEC_ENERGY,
            FIGHT_PROP_MAX_WATER_ENERGY,
            FIGHT_PROP_MAX_GRASS_ENERGY,
            FIGHT_PROP_MAX_WIND_ENERGY,
            FIGHT_PROP_MAX_ICE_ENERGY,
            FIGHT_PROP_MAX_ROCK_ENERGY,
        };

        CompoundProperties = new Dictionary<uint, CompoundProperty>
        {
            [FIGHT_PROP_MAX_HP] = new(FIGHT_PROP_MAX_HP, FIGHT_PROP_BASE_HP, FIGHT_PROP_HP_PERCENT, FIGHT_PROP_HP),
            [FIGHT_PROP_CUR_ATTACK] = new(FIGHT_PROP_CUR_ATTACK, FIGHT_PROP_BASE_ATTACK, FIGHT_PROP_ATTACK_PERCENT, FIGHT_PROP_ATTACK),
            [FIGHT_PROP_CUR_DEFENSE] = new(FIGHT_PROP_CUR_DEFENSE, FIGHT_PROP_BASE_DEFENSE, FIGHT_PROP_DEFENSE_PERCENT, FIGHT_PROP_DEFENSE),
        };
    }

    #region None

    public const uint FIGHT_PROP_NONE = 0;

    #endregion

    #region Base Stats (1-19)

    public const uint FIGHT_PROP_BASE_HP = 1;
    public const uint FIGHT_PROP_HP = 2;
    public const uint FIGHT_PROP_HP_PERCENT = 3;
    public const uint FIGHT_PROP_BASE_ATTACK = 4;
    public const uint FIGHT_PROP_ATTACK = 5;
    public const uint FIGHT_PROP_ATTACK_PERCENT = 6;
    public const uint FIGHT_PROP_BASE_DEFENSE = 7;
    public const uint FIGHT_PROP_DEFENSE = 8;
    public const uint FIGHT_PROP_DEFENSE_PERCENT = 9;
    public const uint FIGHT_PROP_BASE_SPEED = 10;
    public const uint FIGHT_PROP_SPEED_PERCENT = 11;
    public const uint FIGHT_PROP_HP_MP_PERCENT = 12;
    public const uint FIGHT_PROP_ATTACK_MP_PERCENT = 13;

    #endregion

    #region Critical & Combat (20-39)

    public const uint FIGHT_PROP_CRITICAL = 20;
    public const uint FIGHT_PROP_ANTI_CRITICAL = 21;
    public const uint FIGHT_PROP_CRITICAL_HURT = 22;
    public const uint FIGHT_PROP_CHARGE_EFFICIENCY = 23;
    public const uint FIGHT_PROP_ADD_HURT = 24;
    public const uint FIGHT_PROP_SUB_HURT = 25;
    public const uint FIGHT_PROP_HEAL_ADD = 26;
    public const uint FIGHT_PROP_HEALED_ADD = 27;
    public const uint FIGHT_PROP_ELEMENT_MASTERY = 28;
    public const uint FIGHT_PROP_PHYSICAL_SUB_HURT = 29;
    public const uint FIGHT_PROP_PHYSICAL_ADD_HURT = 30;
    public const uint FIGHT_PROP_DEFENCE_IGNORE_RATIO = 31;
    public const uint FIGHT_PROP_DEFENCE_IGNORE_DELTA = 32;

    #endregion

    #region Elemental Damage Bonus (40-49)

    public const uint FIGHT_PROP_FIRE_ADD_HURT = 40;
    public const uint FIGHT_PROP_ELEC_ADD_HURT = 41;
    public const uint FIGHT_PROP_WATER_ADD_HURT = 42;
    public const uint FIGHT_PROP_GRASS_ADD_HURT = 43;
    public const uint FIGHT_PROP_WIND_ADD_HURT = 44;
    public const uint FIGHT_PROP_ROCK_ADD_HURT = 45;
    public const uint FIGHT_PROP_ICE_ADD_HURT = 46;
    public const uint FIGHT_PROP_HIT_HEAD_ADD_HURT = 47;

    #endregion

    #region Elemental Resistance (50-59)

    public const uint FIGHT_PROP_FIRE_SUB_HURT = 50;
    public const uint FIGHT_PROP_ELEC_SUB_HURT = 51;
    public const uint FIGHT_PROP_WATER_SUB_HURT = 52;
    public const uint FIGHT_PROP_GRASS_SUB_HURT = 53;
    public const uint FIGHT_PROP_WIND_SUB_HURT = 54;
    public const uint FIGHT_PROP_ROCK_SUB_HURT = 55;
    public const uint FIGHT_PROP_ICE_SUB_HURT = 56;

    #endregion

    #region Status Resistances (60-69)

    public const uint FIGHT_PROP_EFFECT_HIT = 60;
    public const uint FIGHT_PROP_EFFECT_RESIST = 61;
    public const uint FIGHT_PROP_FREEZE_RESIST = 62;
    public const uint FIGHT_PROP_TORPOR_RESIST = 63;
    public const uint FIGHT_PROP_DIZZY_RESIST = 64;
    public const uint FIGHT_PROP_FREEZE_SHORTEN = 65;
    public const uint FIGHT_PROP_TORPOR_SHORTEN = 66;
    public const uint FIGHT_PROP_DIZZY_SHORTEN = 67;

    #endregion

    #region Energy Max & CD (70-89)

    public const uint FIGHT_PROP_MAX_FIRE_ENERGY = 70;
    public const uint FIGHT_PROP_MAX_ELEC_ENERGY = 71;
    public const uint FIGHT_PROP_MAX_WATER_ENERGY = 72;
    public const uint FIGHT_PROP_MAX_GRASS_ENERGY = 73;
    public const uint FIGHT_PROP_MAX_WIND_ENERGY = 74;
    public const uint FIGHT_PROP_MAX_ICE_ENERGY = 75;
    public const uint FIGHT_PROP_MAX_ROCK_ENERGY = 76;
    public const uint FIGHT_PROP_MAX_SPECIAL_ENERGY = 77;
    public const uint FIGHT_PROP_START_SPECIAL_ENERGY = 78;
    public const uint FIGHT_PROP_SKILL_CD_MINUS_RATIO = 80;
    public const uint FIGHT_PROP_SHIELD_COST_MINUS_RATIO = 81;

    #endregion

    #region Energy Current (1000-1009)

    public const uint FIGHT_PROP_CUR_FIRE_ENERGY = 1000;
    public const uint FIGHT_PROP_CUR_ELEC_ENERGY = 1001;
    public const uint FIGHT_PROP_CUR_WATER_ENERGY = 1002;
    public const uint FIGHT_PROP_CUR_GRASS_ENERGY = 1003;
    public const uint FIGHT_PROP_CUR_WIND_ENERGY = 1004;
    public const uint FIGHT_PROP_CUR_ICE_ENERGY = 1005;
    public const uint FIGHT_PROP_CUR_ROCK_ENERGY = 1006;
    public const uint FIGHT_PROP_CUR_SPECIAL_ENERGY = 1007;

    #endregion

    #region Current Stats (1010-1999)

    public const uint FIGHT_PROP_CUR_HP = 1010;
    public const uint FIGHT_PROP_MIN_SPECIAL_ENERGY = 6969;

    #endregion

    #region Max / Current Stats (2000-2999)

    public const uint FIGHT_PROP_MAX_HP = 2000;
    public const uint FIGHT_PROP_CUR_ATTACK = 2001;
    public const uint FIGHT_PROP_CUR_DEFENSE = 2002;
    public const uint FIGHT_PROP_CUR_SPEED = 2003;
    public const uint FIGHT_PROP_CUR_HP_DEBTS = 2004;
    public const uint FIGHT_PROP_CUR_HP_PAID_DEBTS = 2005;
    public const uint FIGHT_PROP_CUR_NATLAN_HP = 2006;

    #endregion

    #region Non-Extra Stats (3000-3024)

    public const uint FIGHT_PROP_NONEXTRA_ATTACK = 3000;
    public const uint FIGHT_PROP_NONEXTRA_DEFENSE = 3001;
    public const uint FIGHT_PROP_NONEXTRA_CRITICAL = 3002;
    public const uint FIGHT_PROP_NONEXTRA_ANTI_CRITICAL = 3003;
    public const uint FIGHT_PROP_NONEXTRA_CRITICAL_HURT = 3004;
    public const uint FIGHT_PROP_NONEXTRA_CHARGE_EFFICIENCY = 3005;
    public const uint FIGHT_PROP_NONEXTRA_ELEMENT_MASTERY = 3006;
    public const uint FIGHT_PROP_NONEXTRA_PHYSICAL_SUB_HURT = 3007;
    public const uint FIGHT_PROP_NONEXTRA_FIRE_ADD_HURT = 3008;
    public const uint FIGHT_PROP_NONEXTRA_ELEC_ADD_HURT = 3009;
    public const uint FIGHT_PROP_NONEXTRA_WATER_ADD_HURT = 3010;
    public const uint FIGHT_PROP_NONEXTRA_GRASS_ADD_HURT = 3011;
    public const uint FIGHT_PROP_NONEXTRA_WIND_ADD_HURT = 3012;
    public const uint FIGHT_PROP_NONEXTRA_ROCK_ADD_HURT = 3013;
    public const uint FIGHT_PROP_NONEXTRA_ICE_ADD_HURT = 3014;
    public const uint FIGHT_PROP_NONEXTRA_FIRE_SUB_HURT = 3015;
    public const uint FIGHT_PROP_NONEXTRA_ELEC_SUB_HURT = 3016;
    public const uint FIGHT_PROP_NONEXTRA_WATER_SUB_HURT = 3017;
    public const uint FIGHT_PROP_NONEXTRA_GRASS_SUB_HURT = 3018;
    public const uint FIGHT_PROP_NONEXTRA_WIND_SUB_HURT = 3019;
    public const uint FIGHT_PROP_NONEXTRA_ROCK_SUB_HURT = 3020;
    public const uint FIGHT_PROP_NONEXTRA_ICE_SUB_HURT = 3021;
    public const uint FIGHT_PROP_NONEXTRA_SKILL_CD_MINUS_RATIO = 3022;
    public const uint FIGHT_PROP_NONEXTRA_SHIELD_COST_MINUS_RATIO = 3023;
    public const uint FIGHT_PROP_NONEXTRA_PHYSICAL_ADD_HURT = 3024;

    #endregion

    #region Elemental Reaction Crit (3025-3046)

    public const uint FIGHT_PROP_ELEM_REACT_CRITICAL = 3025;
    public const uint FIGHT_PROP_ELEM_REACT_CRITICAL_HURT = 3026;
    public const uint FIGHT_PROP_ELEM_REACT_EXPLODE_CRITICAL = 3027;
    public const uint FIGHT_PROP_ELEM_REACT_EXPLODE_CRITICAL_HURT = 3028;
    public const uint FIGHT_PROP_ELEM_REACT_SWIRL_CRITICAL = 3029;
    public const uint FIGHT_PROP_ELEM_REACT_SWIRL_CRITICAL_HURT = 3030;
    public const uint FIGHT_PROP_ELEM_REACT_ELECTRIC_CRITICAL = 3031;
    public const uint FIGHT_PROP_ELEM_REACT_ELECTRIC_CRITICAL_HURT = 3032;
    public const uint FIGHT_PROP_ELEM_REACT_SCONDUCT_CRITICAL = 3033;
    public const uint FIGHT_PROP_ELEM_REACT_SCONDUCT_CRITICAL_HURT = 3034;
    public const uint FIGHT_PROP_ELEM_REACT_BURN_CRITICAL = 3035;
    public const uint FIGHT_PROP_ELEM_REACT_BURN_CRITICAL_HURT = 3036;
    public const uint FIGHT_PROP_ELEM_REACT_FROZENBROKEN_CRITICAL = 3037;
    public const uint FIGHT_PROP_ELEM_REACT_FROZENBROKEN_CRITICAL_HURT = 3038;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_CRITICAL = 3039;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_CRITICAL_HURT = 3040;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_FIRE_CRITICAL = 3041;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_FIRE_CRITICAL_HURT = 3042;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_ELECTRIC_CRITICAL = 3043;
    public const uint FIGHT_PROP_ELEM_REACT_OVERGROW_ELECTRIC_CRITICAL_HURT = 3044;
    public const uint FIGHT_PROP_BASE_ELEM_REACT_CRITICAL = 3045;
    public const uint FIGHT_PROP_BASE_ELEM_REACT_CRITICAL_HURT = 3046;

    #endregion
}
