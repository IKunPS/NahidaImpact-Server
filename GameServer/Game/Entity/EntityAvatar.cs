using NahidaImpact.Database.Avatar;
using NahidaImpact.Enums;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.World;
using NahidaImpact.Prop;


namespace NahidaImpact.GameServer.Game.Entity;

public class EntityAvatar : BaseEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityAvatar;

    public AvatarDataInfo AvatarInfo { get; }
    public PlayerInstance Player { get; set; }
    
    public uint LastMoveSceneTimeMs { get; set; }
    public uint LastMoveReliableSeq { get; set; }
    public uint LifeState { get; set; }
    
    public EntityAvatar(PlayerInstance player, AvatarDataInfo avatarInfo)
    {
        Owner = player;
        AvatarInfo = avatarInfo;
        Player = player;
        Properties = avatarInfo.Properties;
        FightProperties = avatarInfo.FightProperties;
        LastMoveSceneTimeMs = 0;
        LastMoveReliableSeq = 0;
        LifeState = 1; // Default alive state

        if (Player.Scene != null)
        {
            Id = (uint)Player.Scene.World.GetNextEntityId(EntityIdTypeEnum.Avatar);
        }
    }
    
    public override Position GetPosition()
    {
        return Player.Position;
    }
    
    public override Position GetRotation()
    {
        return Player.Rotation;
    }
    
    public override uint getEntityTypeId()
    {
        return (uint)EntityIdTypeEnum.Avatar;
    }
    
    public override SceneEntityInfo ToProto()
    {
        try
        {
            // Build EntityAuthorityInfo similar to Java implementation
            var authority = new EntityAuthorityInfo
            {
                AbilityInfo = new AbilitySyncStateInfo(),
                RendererChangedInfo = new EntityRendererChangedInfo(),
                AiInfo = new SceneEntityAiInfo(),
                BornPos = new Vector()
            };

            // Build SceneEntityInfo
            var entityInfo = new SceneEntityInfo
            {
                EntityId = Id,
                EntityType = ProtEntityType.ProtEntityAvatar,
                EntityAuthorityInfo = authority,
                LastMoveSceneTimeMs = LastMoveSceneTimeMs,
                LastMoveReliableSeq = LastMoveReliableSeq,
                LifeState = LifeState,
                EntityClientData = new EntityClientData()
            };

            // Add animator parameter list (empty entry like Java)
            entityInfo.AnimatorParaList.Add(new AnimatorParameterValueInfoPair());

            // Set motion info if in scene
            if (Owner?.Scene != null)
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
                PropValue = new PropValue { Type = PlayerProp.PROP_LEVEL, Ival = 1 } // Default level 1
            });

            // Set avatar information
            entityInfo.Avatar = GetSceneAvatarInfo();

            return entityInfo;
        }
        catch (Exception ex)
        {
            // Log error and rethrow or return empty entity info
            // For now, rethrow the exception
            throw new InvalidOperationException("Failed to convert EntityAvatar to proto", ex);
        }
    }

    public SceneAvatarInfo GetSceneAvatarInfo()
    {
        var avatarInfo = AvatarInfo;
        var player = Player;

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

        // Add weapon information
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

        // Add equip ID list
        sceneAvatarInfo.EquipIdList.Add(avatarInfo.WeaponId);

        // TODO: Add reliquary list when equipment system is implemented
        // foreach (var item in avatarInfo.Equips.Values) ...

        return sceneAvatarInfo;
    }

    public AbilityControlBlock GetAbilityControlBlock()
    {
        return new AbilityControlBlock();
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