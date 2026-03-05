using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace NahidaImpact.Data.Ability;

public class AbilityModifierAction
{
    [JsonPropertyName("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    [JsonPropertyName("predicates")]
    public List<object> Predicates { get; set; } = [];

    [JsonPropertyName("amount")]
    public float Amount { get; set; }

    [JsonPropertyName("amountByTargetCurrentHPRatio")]
    public float AmountByTargetCurrentHPRatio { get; set; }

    [JsonPropertyName("amountByCasterAttackRatio")]
    public float AmountByCasterAttackRatio { get; set; }

    [JsonPropertyName("amountByCasterCurrentHPRatio")]
    public float AmountByCasterCurrentHPRatio { get; set; }

    [JsonPropertyName("amountByCasterMaxHPRatio")]
    public float AmountByCasterMaxHPRatio { get; set; }

    [JsonPropertyName("amountByGetDamage")]
    public float AmountByGetDamage { get; set; }

    [JsonPropertyName("amountByTargetMaxHPRatio")]
    public float AmountByTargetMaxHPRatio { get; set; }

    [JsonPropertyName("limboByTargetMaxHPRatio")]
    public float LimboByTargetMaxHPRatio { get; set; }

    [JsonPropertyName("healRatio")]
    public float HealRatio { get; set; } = 1.0f;

    [JsonPropertyName("speed")]
    public float Speed { get; set; } = 1.0f;

    [JsonPropertyName("ignoreAbilityProperty")]
    public bool IgnoreAbilityProperty { get; set; }

    [JsonPropertyName("modifierName")]
    public string ModifierName { get; set; } = string.Empty;

    [JsonPropertyName("enableLockHP")]
    public bool EnableLockHP { get; set; }

    [JsonPropertyName("disableWhenLoading")]
    public bool DisableWhenLoading { get; set; }

    [JsonPropertyName("lethal")]
    public bool Lethal { get; set; } = true;

    [JsonPropertyName("muteHealEffect")]
    public bool MuteHealEffect { get; set; }

    [JsonPropertyName("byServer")]
    public bool ByServer { get; set; }

    [JsonPropertyName("lifeByOwnerIsAlive")]
    public bool LifeByOwnerIsAlive { get; set; }

    [JsonPropertyName("campTargetType")]
    public string CampTargetType { get; set; } = string.Empty;

    [JsonPropertyName("campID")]
    public int CampID { get; set; }

    [JsonPropertyName("gadgetID")]
    public int GadgetID { get; set; }

    [JsonPropertyName("stateID")]
    public int StateID { get; set; }

    [JsonPropertyName("ownerIsTarget")]
    public bool OwnerIsTarget { get; set; }

    [JsonPropertyName("isFromOwner")]
    public bool IsFromOwner { get; set; }

    [JsonPropertyName("healTag")]
    public string HealTag { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("abilityName")]
    public string AbilityName { get; set; } = string.Empty;

    [JsonPropertyName("globalValueKey")]
    public string GlobalValueKey { get; set; } = string.Empty;

    [JsonPropertyName("abilityFormula")]
    public string AbilityFormula { get; set; } = string.Empty;

    [JsonPropertyName("srcTarget")]
    public string SrcTarget { get; set; } = string.Empty;

    [JsonPropertyName("dstTarget")]
    public string DstTarget { get; set; } = string.Empty;

    [JsonPropertyName("srcKey")]
    public string SrcKey { get; set; } = string.Empty;

    [JsonPropertyName("dstKey")]
    public string DstKey { get; set; } = string.Empty;

    [JsonPropertyName("targetPredicates")]
    public List<Dictionary<string, object>> TargetPredicates { get; set; } = [];

    [JsonPropertyName("minValue")]
    public float MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public float MaxValue { get; set; }

    [JsonPropertyName("targetValue")]
    public float TargetValue { get; set; }

    [JsonPropertyName("costStaminaRatio")]
    public float CostStaminaRatio { get; set; }

    [JsonPropertyName("useLimitRange")]
    public bool UseLimitRange { get; set; }

    [JsonPropertyName("skillID")]
    public int SkillID { get; set; }

    [JsonPropertyName("resistanceListID")]
    public int ResistanceListID { get; set; }

    [JsonPropertyName("monsterID")]
    public int MonsterID { get; set; }

    [JsonPropertyName("summonTag")]
    public int SummonTag { get; set; }

    [JsonPropertyName("otherTargets")]
    public AbilityModifierAction OtherTargets { get; set; }

    [JsonPropertyName("actions")]
    public AbilityModifierAction[] Actions { get; set; } = [];

    [JsonPropertyName("successActions")]
    public AbilityModifierAction[] SuccessActions { get; set; } = [];

    [JsonPropertyName("failActions")]
    public AbilityModifierAction[] FailActions { get; set; } = [];

    [JsonPropertyName("dropType")]
    public string DropType { get; set; } = "LevelControl";

    [JsonPropertyName("baseEnergy")]
    public float BaseEnergy { get; set; }

    [JsonPropertyName("ratio")]
    public float Ratio { get; set; } = 1.0f;

    [JsonPropertyName("determineType")]
    public string DetermineType { get; set; } = string.Empty;

    [JsonPropertyName("configID")]
    public int ConfigID { get; set; }

    [JsonPropertyName("valueRangeMin")]
    public float ValueRangeMin { get; set; }

    [JsonPropertyName("valueRangeMax")]
    public float ValueRangeMax { get; set; }

    [JsonPropertyName("overrideMapKey")]
    public string OverrideMapKey { get; set; } = string.Empty;

    [JsonPropertyName("paramNum")]
    public int ParamNum { get; set; }

    [JsonPropertyName("param1")]
    public float Param1 { get; set; }

    [JsonPropertyName("param2")]
    public float Param2 { get; set; }

    [JsonPropertyName("param3")]
    public float Param3 { get; set; }

    [JsonPropertyName("funcName")]
    public string FuncName { get; set; } = string.Empty;

    [JsonPropertyName("luaCallType")]
    public string LuaCallType { get; set; } = string.Empty;

    [JsonPropertyName("CallParamList")]
    public int[] CallParamList { get; set; } = [];

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}