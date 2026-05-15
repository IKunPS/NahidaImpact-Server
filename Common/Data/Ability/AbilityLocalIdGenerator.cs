using System.Collections.Generic;
using NahidaImpact.Util;

namespace NahidaImpact.Data.Ability;

public class AbilityLocalIdGenerator
{
    public enum ConfigAbilitySubContainerType
    {
        NONE = 0,
        ACTION = 1,
        MIXIN = 2,
        MODIFIER_ACTION = 3,
        MODIFIER_MIXIN = 4
    }

    public ConfigAbilitySubContainerType Type { get; set; }
    public long ModifierIndex { get; set; }
    public long ConfigIndex { get; set; }
    public long MixinIndex { get; set; }
    private long _actionIndex;

    public AbilityLocalIdGenerator(ConfigAbilitySubContainerType type)
    {
        Type = type;
    }

    public void InitializeActionLocalIds(
        AbilityModifierAction[] actions,
        Dictionary<int, AbilityModifierAction> localIdToAction)
    {
        InitializeActionLocalIds(actions, localIdToAction, false);
    }

    public void InitializeActionLocalIds(
        AbilityModifierAction[] actions,
        Dictionary<int, AbilityModifierAction> localIdToAction,
        bool preserveActionIndex)
    {
        if (actions == null) return;
        if (!preserveActionIndex) _actionIndex = 0;
        for (int i = 0; i < actions.Length; i++)
        {
            _actionIndex++;

            var id = GetLocalId();
            localIdToAction[(int)id] = actions[i];

            if (actions[i].Actions != null && actions[i].Actions.Length > 0)
                InitializeActionLocalIds(actions[i].Actions, localIdToAction, true);
            else
            {
                if (actions[i].SuccessActions != null && actions[i].SuccessActions.Length > 0)
                    InitializeActionLocalIds(actions[i].SuccessActions, localIdToAction, true);
                if (actions[i].FailActions != null && actions[i].FailActions.Length > 0)
                    InitializeActionLocalIds(actions[i].FailActions, localIdToAction, true);
            }
        }

        if (!preserveActionIndex) _actionIndex = 0;
    }

    public void InitializeMixinsLocalIds(
        AbilityMixinData[] mixins,
        Dictionary<int, AbilityMixinData> localIdToMixin)
    {
        if (mixins == null) return;
        MixinIndex = 0;
        foreach (var mixin in mixins)
        {
            var id = GetLocalId();
            localIdToMixin[(int)id] = mixin;

            MixinIndex++;
        }

        MixinIndex = 0;
    }

    public long GetLocalId()
    {
        switch (Type)
        {
            case ConfigAbilitySubContainerType.ACTION:
                return (long)Type + (ConfigIndex << 3) + (_actionIndex << 9);
            case ConfigAbilitySubContainerType.MIXIN:
                return (long)Type + (MixinIndex << 3) + (ConfigIndex << 9) + (_actionIndex << 15);
            case ConfigAbilitySubContainerType.MODIFIER_ACTION:
                return (long)Type + (ModifierIndex << 3) + (ConfigIndex << 9) + (_actionIndex << 15);
            case ConfigAbilitySubContainerType.MODIFIER_MIXIN:
                return (long)Type
                    + (ModifierIndex << 3)
                    + (MixinIndex << 9)
                    + (ConfigIndex << 15)
                    + (_actionIndex << 21);
            case ConfigAbilitySubContainerType.NONE:
            default:
                return -1;
        }
    }
}
