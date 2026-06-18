using System;
using NahidaImpact.Data;
using NahidaImpact.Data.Binout;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Enums.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Prop;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityAvatar : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityAvatar;

    public AvatarDataInfo AvatarInfo { get; }

    public uint LifeState { get; set; }
    public uint WeaponEntityId => Owner?.WeaponEntityId ?? 0;
    
    public override bool IsAlive()
    {
        return !IsDead && LifeState == 1 && GetFightProperty(FightProp.FIGHT_PROP_CUR_HP) > 0f;
    }
    
    public override Dictionary<int, float> GetFightProperties()
    {
        var dict = new Dictionary<int, float>();
        foreach (var fp in FightProperties)
            dict[(int)fp.PropType] = fp.PropValue;
        return dict;
    }

    /// <summary>Invoked when a global ability value is updated. Checks quest triggers.</summary>
    public override void OnAbilityValueUpdate()
    {
        base.OnAbilityValueUpdate();
        // TODO: Check for _ABILITY_UziExplode_Count quest trigger
    }

    public void SetMotionState(MotionState state)
    {
        MotionState = state;
    }

    private uint _killedType = 0;
    private int _killedBy = 0;
    
    public uint GetKilledType() => _killedType;
    
    public int GetKilledBy() => _killedBy;

    public void SetKilled(uint dieType, int killerId)
    {
        _killedType = dieType;
        _killedBy = killerId;
        LifeState = 0;
    }
    
    public EntityAvatar(Scene scene, AvatarDataInfo avatarInfo) : base(scene)
    {
        Owner = scene.GetHost()!;
        AvatarInfo = avatarInfo;
        Properties = avatarInfo.Properties;
        FightProperties = avatarInfo.FightProperties;
        LastMoveSceneTimeMs = 0;
        LastMoveReliableSeq = 0;
        LifeState = 1;

        if (Scene?.World != null)
        {
            Id = (uint)Scene.World.GetNextEntityId(EntityIdTypeEnum.Avatar);
        }

        InitAbilities();
    }
    
    public void InitAbilities()
    {
        // Get avatar short name from AvatarData (built via BuildEmbryo from IconName)
        var avatarData = GameData.AvatarData.GetValueOrDefault((int)AvatarInfo.AvatarId);
        var avatarName = avatarData?.Name;
        if (string.IsNullOrEmpty(avatarName)) return;

        if (!GameData.AvatarConfigData.TryGetValue(avatarName, out var baseConfig)) return;
        if (baseConfig.Abilities == null) return;

        // Collect base ability names
        var allAbilities = new HashSet<string>();
        foreach (var ab in baseConfig.Abilities)
        {
            if (!string.IsNullOrEmpty(ab.AbilityName))
                allAbilities.Add(ab.AbilityName);
        }

        // TODO: Merge extra ability embryos from equipment, talents, constellations
        // var extraEmbryos = Player.AvatarManager?.GetAvatarByGuid(AvatarInfo.Guid)?.ExtraAbilityEmbryos;
        // if (extraEmbryos != null) { foreach (var skill in extraEmbryos) allAbilities.Add(skill); }

        // If extra abilities were added, update cached config and rebuild avatar data
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

            // Rebuild avatar data abilities from merged list
            if (avatarData != null)
            {
                avatarData.Abilities.Clear();
                avatarData.AbilityNames.Clear();
                foreach (var abilityName in allAbilities)
                {
                    avatarData.Abilities.Add(Utils.AbilityHash(abilityName));
                    avatarData.AbilityNames.Add(abilityName);
                }
            }
        }

        // TODO: Recalculate stats
        // Player.AvatarManager?.GetAvatarById(AvatarInfo.AvatarId)?.RecalcStats(true);
    }
    
    public override Position GetPosition()
    {
        return Owner?.Position ?? new Position();
    }
    
    public override Position GetRotation()
    {
        return Owner?.Rotation ?? new Position();
    }

    public override void Move(Position newPosition, Position rotation)
    {
        // Invoke player move event.
        var evt = new Server.Event.Player.PlayerMoveEvent(
            Owner, Server.Event.Player.PlayerMoveEvent.MoveType.PLAYER, GetPosition(), newPosition);
        evt.Call();

        // Set position and rotation.
        base.Move(evt.GetDestination(), rotation);
    }

    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.Avatar;
    }
    
    public override SceneEntityInfo ToProto()
    {
        try
        {
            var authority = new EntityAuthorityInfo
            {
                AbilityInfo = new AbilitySyncStateInfo(),
                RendererChangedInfo = new EntityRendererChangedInfo(),
                AiInfo = new SceneEntityAiInfo(),
                BornPos = new Vector()
            };
            
            var entityInfo = new SceneEntityInfo
            {
                EntityId = Id,
                EntityType = ProtEntityType.ProtEntityAvatar,
                EntityAuthorityInfo = authority,
                LastMoveSceneTimeMs = (uint)LastMoveSceneTimeMs,
                LastMoveReliableSeq = (uint)LastMoveReliableSeq,
                LifeState = LifeState,
                EntityClientData = new EntityClientData()
            };
            
            entityInfo.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());
            
            if (Scene != null)
            {
                entityInfo.MotionInfo = GetMotionInfo();
            }

            foreach (var fightProp in FightProperties)
            {
                entityInfo.FightPropList.Add(new FightPropPair
                {
                    PropType = fightProp.PropType,
                    PropValue = fightProp.PropValue
                });
            }
            
            entityInfo.PropList.Add(new PropPair 
            { 
                Type = PlayerProp.PROP_LEVEL, 
                PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = 1 }
            });
            
            entityInfo.Avatar = GetSceneAvatarInfo();

            return entityInfo;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to convert EntityAvatar to proto", ex);
        }
    }

    public SceneAvatarInfo GetSceneAvatarInfo()
    {
        var avatarInfo = AvatarInfo;
        var player = Owner;

        var sceneAvatarInfo = new SceneAvatarInfo
        {
            Uid = (uint)player.Uid,
            AvatarId = avatarInfo.AvatarId,
            Guid = avatarInfo.Guid,
            PeerId = (uint)player.PeerId,
            SkillDepotId = avatarInfo.SkillDepotId,
            CoreProudSkillLevel = 0, // TODO: Implement core proud skill level
            WearingFlycloakId = avatarInfo.WearingFlycloakId,
            BornTime = avatarInfo.BornTime,
            CostumeId = 0, // TODO: Implement costume system
            WeaponSkinId = 0, // TODO: Implement weapon skin system
            TraceEffectId = 0 // TODO: Implement trace effect system
        };

        // Add talent ID list (empty for now)
        // sceneAvatarInfo.TalentIdList.AddRange(avatarInfo.TalentIdList);

        // Add skill level map (hardcoded for now - should come from avatar data)
        // TODO: Implement skill level map from avatar data

        // Add inherent proud skill list (hardcoded for now)
        // TODO: Implement proud skill list

        // Add proud skill extra level map (hardcoded for now)
        // TODO: Implement proud skill extra level map

        // Add team resonances from team manager
        if (player.TeamManager != null)
        {
            // TODO: Get actual team resonances
            // For now, keep empty
        }
        
        sceneAvatarInfo.Weapon = new SceneWeaponInfo
        {
            EntityId = player.WeaponEntityId,
            GadgetId = 50000000 + avatarInfo.WeaponId,
            ItemId = avatarInfo.WeaponId,
            Guid = avatarInfo.WeaponGuid,
            Level = 1,
            PromoteLevel = 0,
            AbilityInfo = new AbilitySyncStateInfo()
        };
        
        sceneAvatarInfo.EquipIdList.Add(avatarInfo.WeaponId);

        // TODO: Add reliquary list when equipment system is implemented
        // foreach (var item in avatarInfo.Equips.Values) ...

        return sceneAvatarInfo;
    }

    public AbilityControlBlock GetAbilityControlBlock()
    {
        var block = new AbilityControlBlock();
        int embryoId = 0;

        // 1. Avatar abilities from AvatarData (built via BuildEmbryo from BinOutput/Avatar/*.json)
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

        // 2. Default abilities
        foreach (var hash in GameConstants.DEFAULT_ABILITY_HASHES)
        {
            block.AbilityEmbryoList.Add(new AbilityEmbryo
            {
                AbilityId = (uint)(++embryoId),
                AbilityNameHash = hash,
                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
            });
        }

        // 3. Team resonances
        // TODO: Implement GetTeamResonancesConfig() in TeamManager
        // var resonances = Player.TeamManager?.GetTeamResonancesConfig();
        // if (resonances != null)
        // {
        //     foreach (var id in resonances)
        //     {
        //         block.AbilityEmbryoList.Add(new AbilityEmbryo
        //         {
        //             AbilityId = (uint)(++embryoId),
        //             AbilityNameHash = (uint)id,
        //             AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
        //         });
        //     }
        // }

        // 4. Skill depot abilities
        if (GameData.AvatarSkillDepotData.TryGetValue((int)AvatarInfo.SkillDepotId, out var skillDepot))
        {
            foreach (var hash in skillDepot.AbilityHashes)
            {
                block.AbilityEmbryoList.Add(new AbilityEmbryo
                {
                    AbilityId = (uint)(++embryoId),
                    AbilityNameHash = hash,
                    AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                });
            }
        }

        // 5. Extra ability embryos (equipment, talents, constellations)
        // TODO: Implement ExtraAbilityEmbryos on Avatar/AvatarDataInfo
        // var extraEmbryos = Player.AvatarManager?.GetAvatarByGuid(AvatarInfo.Guid)?.ExtraAbilityEmbryos;
        // if (extraEmbryos != null)
        // {
        //     foreach (var skill in extraEmbryos)
        //     {
        //         block.AbilityEmbryoList.Add(new AbilityEmbryo
        //         {
        //             AbilityId = (uint)(++embryoId),
        //             AbilityNameHash = Utils.AbilityHash(skill),
        //             AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
        //         });
        //     }
        // }

        // 6. Scene LevelEntity config abilities
        var scene = Scene;
        if (scene != null)
        {
            var levelEntityConfig = scene.SceneData?.LevelEntityConfig;
            if (!string.IsNullOrEmpty(levelEntityConfig)
                && GameData.ConfigLevelEntityDataMap.TryGetValue(levelEntityConfig, out var config))
            {
                if (config.AvatarAbilities != null)
                {
                    foreach (var abilityData in config.AvatarAbilities)
                    {
                        if (!string.IsNullOrEmpty(abilityData.AbilityName))
                        {
                            block.AbilityEmbryoList.Add(new AbilityEmbryo
                            {
                                AbilityId = (uint)(++embryoId),
                                AbilityNameHash = Utils.AbilityHash(abilityData.AbilityName),
                                AbilityOverrideNameHash = GameConstants.DEFAULT_ABILITY_NAME
                            });
                        }
                    }
                }
            }
        }

        return block;
    }
    
    public AvatarInfo GetAvatarInfo()
    {

        var avatarInfo = AvatarInfo;
        var proto = new AvatarInfo
        {
            AvatarId = avatarInfo.AvatarId,
            Guid = avatarInfo.Guid,
            BornTime = avatarInfo.BornTime,
            SkillDepotId = avatarInfo.SkillDepotId
        };
        
        // Add weapon information (basic)
        // Note: This is a simplified version to avoid compilation errors
        // TODO: Add proper weapon info and other fields
        return proto;
    }
}