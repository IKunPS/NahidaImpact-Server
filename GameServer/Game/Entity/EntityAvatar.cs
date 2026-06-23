using NahidaImpact.Data;
using NahidaImpact.Data.Binout;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Entity;
using NahidaImpact.Enums.Item;
using NahidaImpact.GameServer.Game.Event.Player;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Prop;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityAvatar : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.Avatar;

    public AvatarDataInfo AvatarInfo { get; }
    public uint LifeState { get; set; } = 1;

    public uint WeaponEntityId
    {
        get
        {
            var weapon = GetEquippedWeaponItem();
            if (weapon != null && weapon.WeaponEntityId > 0)
                return (uint)weapon.WeaponEntityId;
            return 0;
        }
    }

    private uint _killedType;
    private int _killedBy;
    public uint KilledType => _killedType;
    public int KilledBy => _killedBy;

    // Fly state tracking
    private bool _isInFly;
    private long _lastStartFlySceneTimeMs;
    private Position _lastStartFlyPos = new();
    private long _lastStaminaDrainTimeMs;

    // Fall damage tracking
    private Position _lastDropPos = new();
    private long _lastDropTimeMs;
    private float _lastFallSpeed;

    public bool IsInFly => _isInFly;

    public override bool IsAlive()
        => !IsDead && LifeState == 1 && GetFightProperty(FightProp.FIGHT_PROP_CUR_HP) > 0f;

    public override Position Position => Owner?.Position ?? new Position();
    public override Position Rotation => Owner?.Rotation ?? new Position();
    public override uint EntityTypeId => (uint)EntityIdTypeEnum.Avatar;

    public EntityAvatar(Scene scene, AvatarDataInfo avatarInfo) : base(scene)
    {
        Owner = scene.Host!;
        AvatarInfo = avatarInfo;
        Properties = avatarInfo.Properties;
        FightProperties = avatarInfo.FightProperties;
        LifeState = 1;
        _lastDropPos = scene.Host?.Position?.Clone() ?? new Position();

        if (Scene?.World != null)
            Id = (uint)Scene.World.GetNextEntityId(EntityIdTypeEnum.Avatar);

        InitAbilities();
        CreateWeaponEntity();
    }

    public void SetMotionState(MotionState state)
    {
        var oldState = MotionState;
        MotionState = state;
        SetIsInFly(state, oldState);

        // Track drop start position for fall damage calculation
        if (state is MotionState.Jump or MotionState.Drop or MotionState.Fly
            or MotionState.FlyIdle or MotionState.FlySlow or MotionState.FlyFast)
        {
            _lastDropPos = Position.Clone();
            _lastDropTimeMs = Scene?.SceneTime ?? 0;
            _lastFallSpeed = 0;
        }

        if (state == MotionState.FallOnGround && _lastDropTimeMs > 0)
            HandleFallDamage();
    }

    private void SetIsInFly(MotionState newState, MotionState oldState)
    {
        var wasFlying = _isInFly;
        _isInFly = newState is MotionState.Fly or MotionState.FlyIdle
            or MotionState.FlySlow or MotionState.FlyFast;

        if (_isInFly && !wasFlying)
        {
            _lastStartFlySceneTimeMs = Scene?.SceneTime ?? 0;
            _lastStartFlyPos = Position.Clone();
            new PlayerFlyEvent(Owner!, this, true).Call();
        }
        else if (wasFlying && !_isInFly)
        {
            var flyTimeMs = (Scene?.SceneTime ?? 0) - _lastStartFlySceneTimeMs;
            var flyTime = flyTimeMs / 1000f;
            var distance = Position.ComputeDistance(_lastStartFlyPos);
            new PlayerFlyEvent(Owner!, this, false, flyTime, distance, _lastStartFlyPos).Call();
        }
    }

    private void HandleFallDamage()
    {
        var sceneTime = Scene?.SceneTime ?? 0;
        if (sceneTime <= _lastDropTimeMs) return;

        var dropDistance = _lastDropPos.Y - Position.Y;
        var dropDuration = (sceneTime - _lastDropTimeMs) / 1000f;
        if (dropDuration <= 0) return;

        _lastFallSpeed = Math.Abs(dropDistance) / dropDuration;

        float minSpeed = 24f;
        float addSpeed = 1f;
        if (GameData.ConstValueMap.TryGetValue("CONST_VALUE_FALL_HURT", out var fallHurt) && fallHurt.Value.Count >= 4)
        {
            minSpeed = Math.Abs(float.Parse(fallHurt.Value[0]));
            addSpeed = Math.Abs(float.Parse(fallHurt.Value[2]));
        }

        var maxHp = GetFightProperty(FightProp.FIGHT_PROP_CUR_HP);
        if (_lastFallSpeed > minSpeed)
        {
            var damage = (_lastFallSpeed - minSpeed) * addSpeed * maxHp * GameConstants.FALL_DAMAGE_RATIO;
            Damage(damage, 0, Data.Ability.ElementType.None);
        }
        _lastDropTimeMs = 0;
    }

    public void SetKilled(uint dieType, int killerId)
    {
        _killedType = dieType;
        _killedBy = killerId;
        LifeState = 0;
        IsDead = true;
    }

    public override void OnAbilityValueUpdate()
    {
        base.OnAbilityValueUpdate();
        // TODO: Quest trigger check for _ABILITY_UziExplode_Count
    }

    public override void OnTick(int sceneTime)
    {
        base.OnTick(sceneTime);
        ProcStaminaDrain();
    }

    private void ProcStaminaDrain()
    {
        if (!_isInFly || Owner == null) return;

        var sceneTime = Scene?.SceneTime ?? 0;
        if (sceneTime - _lastStaminaDrainTimeMs < GameConstants.STAMINA_TICK_INTERVAL_MS) return;
        _lastStaminaDrainTimeMs = sceneTime;

        var flyCostPerSec = ConstValue.GetUint("CONST_VALUE_FLY_COST_STAMINA");
        var tickCost = flyCostPerSec * GameConstants.STAMINA_TICK_RATIO;

        if (!Owner.ConsumeStamina(tickCost))
        {
            // Stamina exhausted — client will auto-transition out of fly
            _isInFly = false;
        }
    }

    private void CreateWeaponEntity()
    {
        var weapon = GetEquippedWeaponItem();
        if (weapon == null || weapon.WeaponEntityId > 0) return;

        var scene = Scene;
        if (scene == null) return;

        var gadgetId = (int)(weapon.ItemDataExcel?.GadgetId ?? 0);
        if (gadgetId <= 0) return;

        var weaponEntity = new EntityWeapon(scene, gadgetId)
        {
            ItemId = weapon.ItemId,
            ItemGuid = weapon.Guid
        };
        weapon.WeaponEntityId = (int)weaponEntity.Id;
        scene.WeaponEntities[(int)weaponEntity.Id] = weaponEntity;
    }

    private ItemData? GetEquippedWeaponItem()
    {
        return Owner?.InventoryManager.Items.Values.FirstOrDefault(i =>
            i.EquipCharacter == (int)AvatarInfo.AvatarId &&
            i.ItemType == ItemType.ITEM_WEAPON);
    }

    private void InitAbilities()
    {
        var avatarData = GameData.AvatarData.GetValueOrDefault((int)AvatarInfo.AvatarId);
        var avatarName = avatarData?.Name;
        if (string.IsNullOrEmpty(avatarName)) return;

        if (!GameData.AvatarConfigData.TryGetValue(avatarName, out var baseConfig)) return;
        if (baseConfig.Abilities == null) return;

        var allAbilities = new HashSet<string>();
        foreach (var ab in baseConfig.Abilities)
        {
            if (!string.IsNullOrEmpty(ab.AbilityName))
                allAbilities.Add(ab.AbilityName);
        }

        // TODO: merge extra ability embryos from equipment, talents, constellations

        var currentAbilities = new HashSet<string>();
        foreach (var ab in baseConfig.Abilities)
        {
            if (!string.IsNullOrEmpty(ab.AbilityName))
                currentAbilities.Add(ab.AbilityName);
        }

        if (!allAbilities.SetEquals(currentAbilities))
        {
            var mergedAbilities = new List<ConfigAbilityData>();
            foreach (var name in allAbilities)
                mergedAbilities.Add(new ConfigAbilityData { AbilityName = name });

            GameData.AvatarConfigData[avatarName] = new ConfigEntityAvatar
            {
                ConfigCommon = baseConfig.ConfigCommon,
                Combat = baseConfig.Combat,
                GlobalValue = baseConfig.GlobalValue,
                Abilities = mergedAbilities
            };

            if (avatarData != null)
            {
                avatarData.Abilities.Clear();
                avatarData.AbilityNames.Clear();
                foreach (var name in allAbilities)
                {
                    avatarData.Abilities.Add(Utils.AbilityHash(name));
                    avatarData.AbilityNames.Add(name);
                }
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

    public override void Move(Position newPosition, Position rotation)
    {
        var evt = new PlayerMoveEvent(Owner, PlayerMoveEvent.MoveType.PLAYER, Position, newPosition);
        evt.Call();
        base.Move(evt.To, rotation);
    }

    #region Proto

    public override SceneEntityInfo ToProto()
    {
        var info = new SceneEntityInfo
        {
            EntityId = Id,
            EntityType = ProtEntityType.Avatar,
            EntityAuthorityInfo = new EntityAuthorityInfo
            {
                AbilityInfo = new AbilitySyncStateInfo(),
                RendererChangedInfo = new EntityRendererChangedInfo(),
                AiInfo = new SceneEntityAiInfo(),
                BornPos = new Vector()
            },
            LastMoveSceneTimeMs = (uint)LastMoveSceneTimeMs,
            LastMoveReliableSeq = (uint)LastMoveReliableSeq,
            LifeState = LifeState,
            EntityClientData = new EntityClientData()
        };

        info.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());

        if (Scene != null)
            info.MotionInfo = GetMotionInfo();

        foreach (var fp in FightProperties)
            info.FightPropList.Add(new FightPropPair { PropType = fp.PropType, PropValue = fp.PropValue });

        info.PropList.Add(new PropPair
        {
            Type = PlayerProp.PROP_LEVEL,
            PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = AvatarInfo.Level }
        });

        info.Avatar = GetSceneAvatarInfo();
        return info;
    }

    public SceneAvatarInfo GetSceneAvatarInfo()
    {
        var player = Owner;
        var sceneInfo = new SceneAvatarInfo
        {
            Uid = (uint)player.Uid,
            AvatarId = AvatarInfo.AvatarId,
            Guid = AvatarInfo.Guid,
            PeerId = (uint)player.PeerId,
            SkillDepotId = AvatarInfo.SkillDepotId,
            CoreProudSkillLevel = AvatarInfo.CoreProudSkillLevel,
            WearingFlycloakId = AvatarInfo.WearingFlycloakId,
            BornTime = AvatarInfo.BornTime,
            CostumeId = AvatarInfo.CostumeId,
            WeaponSkinId = AvatarInfo.WeaponSkinId,
            TraceEffectId = AvatarInfo.TraceEffectId,
        };

        foreach (var talentId in AvatarInfo.TalentIdList)
            sceneInfo.TalentIdList.Add(talentId);

        foreach (var kv in AvatarInfo.SkillLevelMap)
            sceneInfo.SkillLevelMap[kv.Key] = kv.Value;

        foreach (var id in AvatarInfo.ProudSkillList)
            sceneInfo.InherentProudSkillList.Add(id);

        foreach (var kv in AvatarInfo.ProudSkillExtraLevelMap)
            sceneInfo.ProudSkillExtraLevelMap[kv.Key] = kv.Value;

        // TODO: team resonances via TeamManager.GetTeamResonances()

        // Weapon
        var weaponItem = GetEquippedWeaponItem();
        if (weaponItem != null)
        {
            sceneInfo.Weapon = weaponItem.CreateSceneWeaponInfo(WeaponEntityId);
            sceneInfo.Weapon.AbilityInfo = new AbilitySyncStateInfo { IsInited = weaponItem.Affixes.Count > 0 };
        }
        else
        {
            sceneInfo.Weapon = new SceneWeaponInfo
            {
                EntityId = player.WeaponEntityId,
                GadgetId = 50000000 + AvatarInfo.WeaponId,
                ItemId = AvatarInfo.WeaponId,
                Guid = AvatarInfo.WeaponGuid,
                Level = 1,
                PromoteLevel = 0,
                AbilityInfo = new AbilitySyncStateInfo()
            };
        }

        sceneInfo.EquipIdList.Add(AvatarInfo.WeaponId);

        var resonances = player.TeamManager.GetTeamResonances();
        sceneInfo.TeamResonanceList.AddRange(resonances);

        // Reliquary equips
        var inventory = player.InventoryManager;
        foreach (var item in inventory.Items.Values)
        {
            if (item.EquipCharacter == (int)AvatarInfo.AvatarId && item.ItemType == ItemType.ITEM_RELIQUARY)
            {
                sceneInfo.EquipIdList.Add((uint)item.ItemId);
                sceneInfo.ReliquaryList.Add(item.CreateSceneReliquaryInfo());
            }
        }

        return sceneInfo;
    }

    public AbilityControlBlock GetAbilityControlBlock()
    {
        var block = new AbilityControlBlock();
        int embryoId = 0;

        // Avatar abilities from AvatarData
        if (GameData.AvatarData.TryGetValue((int)AvatarInfo.AvatarId, out var avatarData))
        {
            foreach (var hash in avatarData.Abilities)
            {
                block.AbilityEmbryoList.Add(new AbilityEmbryo
                {
                    AbilityId = (uint)(++embryoId),
                    AbilityNameHash = hash,
                    AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                });
            }
        }

        // Default abilities
        foreach (var hash in GameConstants.DEFAULT_ABILITY_HASHES)
        {
            block.AbilityEmbryoList.Add(new AbilityEmbryo
            {
                AbilityId = (uint)(++embryoId),
                AbilityNameHash = hash,
                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
            });
        }

        // Team resonances
        foreach (var id in Owner.TeamManager.GetTeamResonances())
        {
            block.AbilityEmbryoList.Add(new AbilityEmbryo
            {
                AbilityId = (uint)(++embryoId),
                AbilityNameHash = (uint)id,
                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
            });
        }

        // Skill depot abilities
        if (GameData.AvatarSkillDepotData.TryGetValue((int)AvatarInfo.SkillDepotId, out var depot))
        {
            foreach (var hash in depot.AbilityHashes)
            {
                block.AbilityEmbryoList.Add(new AbilityEmbryo
                {
                    AbilityId = (uint)(++embryoId),
                    AbilityNameHash = hash,
                    AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                });
            }
        }

        // Level entity config abilities
        var scene = Scene;
        if (scene != null)
        {
            var levelConfig = scene.SceneData?.LevelEntityConfig;
            if (!string.IsNullOrEmpty(levelConfig)
                && GameData.ConfigLevelEntityDataMap.TryGetValue(levelConfig, out var config))
            {
                if (config.AvatarAbilities != null)
                {
                    foreach (var a in config.AvatarAbilities)
                    {
                        if (!string.IsNullOrEmpty(a.AbilityName))
                        {
                            block.AbilityEmbryoList.Add(new AbilityEmbryo
                            {
                                AbilityId = (uint)(++embryoId),
                                AbilityNameHash = Utils.AbilityHash(a.AbilityName),
                                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                            });
                        }
                    }
                }
            }
        }

        return block;
    }

    #endregion
}
