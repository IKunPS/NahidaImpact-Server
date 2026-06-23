using NahidaImpact.Data.Ability;
using NahidaImpact.GameServer.Game.Ability;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send;
using NahidaImpact.Prop;
using NahidaImpact.Proto;
using NahidaImpact.GameServer.Server.Packet.Send.Entity;
using ElementType = NahidaImpact.Data.Ability.ElementType;

namespace NahidaImpact.GameServer.Game.Entity;

public abstract class BaseEntity
{
    public abstract ProtEntityType EntityType { get; }
    public uint Id { get; set; }
    public Scene Scene { get; }
    public PlayerInstance Owner { get; set; }
    public MotionState MotionState { get; set; }
    public int LastMoveSceneTimeMs { get; set; }
    public int LastMoveReliableSeq { get; set; }
    public List<PropValue> Properties { get; set; }
    public List<FightPropPair> FightProperties { get; set; }

    public List<Ability.Ability> InstancedAbilities { get; set; } = new();
    public Dictionary<int, AbilityModifierController> InstancedModifiers { get; set; } = new();
    public Dictionary<string, float> GlobalAbilityValues { get; set; } = new();
    
    public bool LockHP { get; set; }
    public bool IsDead { get; set; }
    public bool RestrictedFromHealing { get; set; }
    public DetailAbilityInfo? DetailAbilityInfo { get; set; }
    public ElementType LastAttackType { get; set; } = ElementType.None;

    public int CampId { get; set; }
    public int CampType { get; set; }
    public int BlockId { get; set; }
    public int ConfigId { get; set; }
    public int GroupId { get; set; }

    private bool _limbo;
    private float _limboHpThreshold;

    public abstract Position Position { get; }
    public abstract Position Rotation { get; }

    public BaseEntity(Scene scene)
    {
        Scene = scene;
        MotionState = MotionState.None;
        Properties = new();
        FightProperties = new();
    }

    public abstract uint EntityTypeId { get; }
    public abstract SceneEntityInfo ToProto();
    
    public abstract Dictionary<int, float> GetFightProperties();

    public virtual MotionInfo GetMotionInfo()
    {
        var motionInfo = new MotionInfo
        {
            Pos = Position.ToProto(),
            Rot = Rotation.ToProto(),
            Speed = new Vector(),
            State = MotionState
        };
        return motionInfo;
    }

    public virtual void Move(Position position, Position rotation)
    {
        Position.Set(position);
        Rotation.Set(rotation);
    }

    public virtual void OnCreate() { }
    public virtual void OnRemoved() { }

    public float GetFightProperty(uint propType)
    {
        var pair = FightProperties.Find(p => p.PropType == propType);
        return pair?.PropValue ?? 0f;
    }

    public void SetFightProperty(int id, float value)
    {
        var pair = FightProperties.Find(p => p.PropType == (uint)id);
        if (pair != null)
        {
            pair.PropValue = value;
            return;
        }
        FightProperties.Add(new FightPropPair { PropType = (uint)id, PropValue = value });
    }

    public void AddFightProperty(uint propType, float value)
    {
        SetFightProperty((int)propType, GetFightProperty(propType) + value);
    }

    public bool HasFightProperty(uint propType)
    {
        return FightProperties.Exists(p => p.PropType == propType);
    }

    public virtual bool IsAlive()
    {
        return !IsDead;
    }

    public float NyxValue => GlobalAbilityValues.TryGetValue("NyxValue", out var val) ? val : 0f;

    public void ClearLimbo()
    {
        _limbo = false;
        _limboHpThreshold = 0f;
    }

    public void SetLimbo(float hpThreshold)
    {
        _limbo = true;
        _limboHpThreshold = hpThreshold;
    }
    
    public virtual void OnAddAbilityModifier(AbilityModifier data)
    {
        if (data.Properties == null) return;

        float hpThresholdRatio = data.Properties.ActorHpThresholdRatio;
        if (data.State == State.Limbo && hpThresholdRatio > 0.0f)
        {
            SetLimbo(hpThresholdRatio);
        }
    }
    
    public virtual float Heal(float amount, bool mute = false)
    {
        if (FightProperties == null) return 0f;

        float toHeal = 0f;
        float toRepay = 0f;
        float curHp = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        float maxHp = GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
        float curHpDebt = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS);

        if (curHp >= maxHp && curHpDebt <= 0) return 0f;

        toRepay = Math.Min(amount, curHpDebt);
        toHeal = Math.Min(maxHp - curHp, amount - toRepay);
        AddFightProperty(FightProp.FIGHT_PROP_CUR_HP, toHeal);
        AddFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS, -toRepay);

        TriggerModifierEvent(m => m.OnHeal);

        if (toHeal > 0)
        {
            Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_HP));
        }
        if (toRepay > 0)
        {
            Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_HP_DEBTS));
            var debtsReason = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS) > 0
                ? ChangeHpDebtsReason.Pay
                : ChangeHpDebtsReason.PayFinish;
            Scene?.BroadcastPacket(new PacketEntityFightPropChangeReasonNotify(
                this, FightProp.FIGHT_PROP_CUR_HP_DEBTS, -toRepay,
                mute ? PropChangeReason.None : PropChangeReason.Ability,
                debtsReason));
        }

        return toHeal;
    }
    
    // hk4e: modify damage ratio set by ModifyDamageMixin, checked in Damage()
    public float ModifyDamageRatio
    {
        get
        {
            if (GlobalAbilityValues.TryGetValue("_ModifyDamage_Ratio", out var value))
                return value;
            return 1f;
        }
    }

    public virtual void Damage(float amount, int killerId = 0, ElementType attackType = ElementType.None)
    {
        if (FightProperties == null || !HasFightProperty(FightProp.FIGHT_PROP_CUR_HP))
            return;

        // Apply damage modification from modifier mixins
        var modifiedAmount = amount * ModifyDamageRatio;

        float effectiveDamage = 0;
        float curHp = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);

        if (_limbo)
        {
            float maxHp = GetFightProperty(FightProp.FIGHT_PROP_MAX_HP);
            float curRatio = curHp / maxHp;
            if (curRatio > _limboHpThreshold)
            {
                effectiveDamage = modifiedAmount;
            }
            if (effectiveDamage >= curHp && _limboHpThreshold > 0f)
            {
                effectiveDamage = curHp - 1;
            }
        }
        else if (curHp != float.PositiveInfinity && !LockHP
            || (LockHP && curHp <= modifiedAmount))
        {
            effectiveDamage = modifiedAmount;
        }

        AddFightProperty(FightProp.FIGHT_PROP_CUR_HP, -effectiveDamage);

        LastAttackType = attackType;
        TriggerModifierEvent(m => m.OnBeingHit);
        CheckIfDead();

        Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_HP));
        Scene?.BroadcastPacket(new PacketEntityFightPropChangeReasonNotify(
            this, FightProp.FIGHT_PROP_CUR_HP, -effectiveDamage,
            PropChangeReason.Ability, ChangeHpReason.SubAbility));

        if (IsDead)
        {
            OnDeath(killerId);
        }
    }

    public virtual void AddSpecialEnergy(float energy)
    {
        float curSpecialEnergy = GetFightProperty(FightProp.FIGHT_PROP_CUR_SPECIAL_ENERGY);
        float maxSpecialEnergy = GetFightProperty(FightProp.FIGHT_PROP_MAX_SPECIAL_ENERGY);
        curSpecialEnergy += energy;
        if (curSpecialEnergy >= maxSpecialEnergy) curSpecialEnergy = maxSpecialEnergy;
        if (curSpecialEnergy <= 0) curSpecialEnergy = 0;
        SetFightProperty((int)FightProp.FIGHT_PROP_CUR_SPECIAL_ENERGY, curSpecialEnergy);
        Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_SPECIAL_ENERGY));
    }

    public virtual void ClearSpecialEnergy()
    {
        SetFightProperty((int)FightProp.FIGHT_PROP_CUR_SPECIAL_ENERGY, 0);
        Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_SPECIAL_ENERGY));
    }
    
    public virtual void CheckIfDead()
    {
        if (FightProperties == null || !HasFightProperty(FightProp.FIGHT_PROP_CUR_HP))
            return;

        if (GetFightProperty(FightProp.FIGHT_PROP_CUR_HP) <= 0f)
        {
            SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP, 0f);
            float debt = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP_DEBTS);
            if (debt >= 0)
            {
                SetFightProperty((int)FightProp.FIGHT_PROP_CUR_HP_DEBTS, 0f);
                Scene?.BroadcastPacket(new PacketEntityFightPropUpdateNotify(this, FightProp.FIGHT_PROP_CUR_HP_DEBTS));
                Scene?.BroadcastPacket(new PacketEntityFightPropChangeReasonNotify(
                    this, FightProp.FIGHT_PROP_CUR_HP_DEBTS, -debt,
                    PropChangeReason.Ability, ChangeHpDebtsReason.Clear));
            }
            IsDead = true;
        }
    }
    
    public virtual void OnAbilityValueUpdate() { }
    
    public virtual void OnDeath(int killerId)
    {
        TriggerModifierEvent(m => m.OnKill);
        IsDead = true;
    }

    // Iterates instanced modifiers and fires matching lifecycle event actions
    protected void TriggerModifierEvent(Func<AbilityModifier, AbilityModifierAction[]> eventSelector)
    {
        foreach (var modCtrl in InstancedModifiers.Values)
        {
            var actions = eventSelector(modCtrl.ModifierData);
            if (actions == null) continue;
            // Lifecycle actions dispatched via the owning ability's manager
            foreach (var action in actions)
            {
                if (modCtrl.Ability?.Manager != null && modCtrl.Ability != null)
                    modCtrl.Ability.Manager.ExecuteAction(modCtrl.Ability, action, Google.Protobuf.ByteString.Empty, this);
            }
        }
    }

    public virtual void OnInteract(PlayerInstance player, object interactReq) { }

    public virtual void OnTick(int sceneTime) { }

    public virtual int OnClientExecuteRequest(int param1, int param2, int param3) => 0;

    public virtual void RunLuaCallbacks(object damageEvent) { }
    
    public void AddAllFightPropsToEntityInfo(SceneEntityInfo entityInfo)
    {
        foreach (var kv in GetFightProperties())
        {
            if (kv.Key == 0) continue;
            entityInfo.FightPropList.Add(new FightPropPair
            {
                PropType = (uint)kv.Key,
                PropValue = kv.Value
            });
        }
    }

    public override string ToString()
    {
        return $"Entity ID: {Id}; Group ID: {GroupId}; Config ID: {ConfigId}";
    }
}
