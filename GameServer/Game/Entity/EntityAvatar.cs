using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Game.Scene;

namespace NahidaImpact.GameServer.Game.Entity;

public class EntityAvatar : SceneEntity
{
    public override ProtEntityType EntityType => ProtEntityType.ProtEntityAvatar;
    public AvatarDataInfo AvatarInfo { get; }
    public PlayerInstance Player { get; set; }
    
    public EntityAvatar(PlayerInstance player, AvatarDataInfo avatarInfo, uint entityId) : base(entityId)
    {
        AvatarInfo = avatarInfo;
        Player = player;
        Properties = avatarInfo.Properties;
        FightProperties = avatarInfo.FightProperties;
    }
    
    public override SceneEntityInfo ToProto()
    {
        SceneEntityInfo info = base.ToProto();

        info.Avatar = new()
        {
            Uid = (uint)Player.Uid,
            AvatarId = AvatarInfo.AvatarId,
            Guid = AvatarInfo.Guid,
            PeerId = 1,
            EquipIdList = { AvatarInfo.WeaponId },
            SkillDepotId = AvatarInfo.SkillDepotId,
            Weapon = new SceneWeaponInfo
            {
                EntityId = Player.WeaponEntityId,
                GadgetId = 50000000 + AvatarInfo.WeaponId,
                ItemId = AvatarInfo.WeaponId,
                Guid = AvatarInfo.WeaponGuid,
                Level = 1,
                PromoteLevel = 0,
                AbilityInfo = new()
            },
            CoreProudSkillLevel = 0,
            InherentProudSkillList = { 832301 },
            SkillLevelMap =
            {
                { 10832, 1 },
                { 10835, 1 },
                { 10831, 1 }
            },
            ProudSkillExtraLevelMap =
            {
                { 8331, 0 },
                { 8332, 0 },
                { 8339, 0 }
            },
            TeamResonanceList = { 10301 },
            WearingFlycloakId = AvatarInfo.WearingFlycloakId,
            BornTime = AvatarInfo.BornTime,
            CostumeId = 0,
            AnimHash = 0
        };

        return info;
    }

    public AbilityControlBlock BuildAbilityControlBlock()
    {
        return new AbilityControlBlock();
    }
}