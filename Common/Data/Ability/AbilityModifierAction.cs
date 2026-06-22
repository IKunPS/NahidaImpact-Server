using Newtonsoft.Json;
using System.Collections.Generic;

namespace NahidaImpact.Data.Ability;

public class AbilityModifierAction
{
    [JsonProperty("$type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("target")]
    public string Target { get; set; } = string.Empty;

    [JsonProperty("predicates")]
    public List<object> Predicates { get; set; } = [];

    [JsonProperty("amount")]
    public float Amount { get; set; }

    [JsonProperty("amountByTargetCurrentHPRatio")]
    public float AmountByTargetCurrentHPRatio { get; set; }

    [JsonProperty("amountByCasterAttackRatio")]
    public float AmountByCasterAttackRatio { get; set; }

    [JsonProperty("amountByCasterCurrentHPRatio")]
    public float AmountByCasterCurrentHPRatio { get; set; }

    [JsonProperty("amountByCasterMaxHPRatio")]
    public float AmountByCasterMaxHPRatio { get; set; }

    [JsonProperty("amountByGetDamage")]
    public float AmountByGetDamage { get; set; }

    [JsonProperty("amountByTargetMaxHPRatio")]
    public float AmountByTargetMaxHPRatio { get; set; }

    [JsonProperty("limboByTargetMaxHPRatio")]
    public float LimboByTargetMaxHPRatio { get; set; }

    [JsonProperty("healRatio")]
    public float HealRatio { get; set; } = 1.0f;

    [JsonProperty("speed")]
    public float Speed { get; set; } = 1.0f;

    [JsonProperty("ignoreAbilityProperty")]
    public bool IgnoreAbilityProperty { get; set; }

    [JsonProperty("modifierName")]
    public string ModifierName { get; set; } = string.Empty;

    [JsonProperty("enableLockHP")]
    public bool EnableLockHP { get; set; }

    [JsonProperty("disableWhenLoading")]
    public bool DisableWhenLoading { get; set; }

    [JsonProperty("lethal")]
    public bool Lethal { get; set; } = true;

    [JsonProperty("muteHealEffect")]
    public bool MuteHealEffect { get; set; }

    [JsonProperty("byServer")]
    public bool ByServer { get; set; }

    [JsonProperty("lifeByOwnerIsAlive")]
    public bool LifeByOwnerIsAlive { get; set; }

    [JsonProperty("campTargetType")]
    public string CampTargetType { get; set; } = string.Empty;

    [JsonProperty("campID")]
    public int CampID { get; set; }

    [JsonProperty("gadgetID")]
    public int GadgetID { get; set; }

    [JsonProperty("stateID")]
    public int StateID { get; set; }

    [JsonProperty("ownerIsTarget")]
    public bool OwnerIsTarget { get; set; }

    [JsonProperty("isFromOwner")]
    public bool IsFromOwner { get; set; }

    [JsonProperty("healTag")]
    public string HealTag { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("abilityName")]
    public string AbilityName { get; set; } = string.Empty;

    [JsonProperty("globalValueKey")]
    public string GlobalValueKey { get; set; } = string.Empty;

    [JsonProperty("abilityFormula")]
    public string AbilityFormula { get; set; } = string.Empty;

    [JsonProperty("srcTarget")]
    public string SrcTarget { get; set; } = string.Empty;

    [JsonProperty("dstTarget")]
    public string DstTarget { get; set; } = string.Empty;

    [JsonProperty("srcKey")]
    public string SrcKey { get; set; } = string.Empty;

    [JsonProperty("dstKey")]
    public string DstKey { get; set; } = string.Empty;

    [JsonProperty("targetPredicates")]
    public List<Dictionary<string, object>> TargetPredicates { get; set; } = [];

    [JsonProperty("minValue")]
    public float MinValue { get; set; }

    [JsonProperty("maxValue")]
    public float MaxValue { get; set; }

    [JsonProperty("targetValue")]
    public float TargetValue { get; set; }

    [JsonProperty("costStaminaRatio")]
    public float CostStaminaRatio { get; set; }

    [JsonProperty("useLimitRange")]
    public bool UseLimitRange { get; set; }

    [JsonProperty("skillID")]
    public int SkillID { get; set; }

    [JsonProperty("resistanceListID")]
    public int ResistanceListID { get; set; }

    [JsonProperty("monsterID")]
    public int MonsterID { get; set; }

    [JsonProperty("summonTag")]
    public int SummonTag { get; set; }

    [JsonProperty("otherTargets")]
    public AbilityModifierAction OtherTargets { get; set; }

    [JsonProperty("actions")]
    public AbilityModifierAction[] Actions { get; set; } = [];

    [JsonProperty("successActions")]
    public AbilityModifierAction[] SuccessActions { get; set; } = [];

    [JsonProperty("failActions")]
    public AbilityModifierAction[] FailActions { get; set; } = [];

    [JsonProperty("dropType")]
    public string DropType { get; set; } = "LevelControl";

    [JsonProperty("baseEnergy")]
    public float BaseEnergy { get; set; }

    [JsonProperty("ratio")]
    public float Ratio { get; set; } = 1.0f;

    [JsonProperty("determineType")]
    public string DetermineType { get; set; } = string.Empty;

    [JsonProperty("configID")]
    public int ConfigID { get; set; }

    [JsonProperty("valueRangeMin")]
    public float ValueRangeMin { get; set; }

    [JsonProperty("valueRangeMax")]
    public float ValueRangeMax { get; set; }

    [JsonProperty("overrideMapKey")]
    public string OverrideMapKey { get; set; } = string.Empty;

    [JsonProperty("paramNum")]
    public int ParamNum { get; set; }

    [JsonProperty("param1")]
    public float Param1 { get; set; }

    [JsonProperty("param2")]
    public float Param2 { get; set; }

    [JsonProperty("param3")]
    public float Param3 { get; set; }

    [JsonProperty("funcName")]
    public string FuncName { get; set; } = string.Empty;

    [JsonProperty("luaCallType")]
    public string LuaCallType { get; set; } = string.Empty;

    [JsonProperty("CallParamList")]
    public int[] CallParamList { get; set; } = [];

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}