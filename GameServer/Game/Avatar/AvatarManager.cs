using NahidaImpact.Data;
using NahidaImpact.Data.Common;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Database.Team;
using NahidaImpact.Enums.Item;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.Proto;
using NahidaImpact.Prop;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Game.Avatar;

public class AvatarManager(PlayerInstance player) : BasePlayerManager(player)
{
    public AvatarData AvatarData { get; } = DatabaseHelper.GetInstanceOrCreateNew<AvatarData>(player.Uid);

    public List<AvatarDataInfo> Avatars => AvatarData.Avatars;

    public int AvatarCount => Avatars.Count;

    public AvatarDataInfo? GetAvatar(int avatarId)
        => Avatars.Find(a => a.AvatarId == avatarId);

    public AvatarDataInfo? GetAvatarById(uint avatarId)
        => Avatars.Find(a => a.AvatarId == avatarId);

    public AvatarDataInfo? GetAvatarByGuid(ulong guid)
        => Avatars.Find(a => a.Guid == guid);

    public bool HasAvatar(int avatarId)
        => Avatars.Any(a => a.AvatarId == avatarId);

    public ulong NextGuid()
        => ((ulong)Player.Uid << 32) + (++Player.GuidSeed);

    public TeamInfo GetCurrentTeam()
    {
        if (Player.TeamManager != null)
            return Player.TeamManager.CurrentTeamInfo;
        return new TeamInfo { Index = 1 };
    }

    public void Save()
    {
        AvatarData.SaveAvatarData(AvatarData);
    }

    #region Avatar Creation

    /// <summary>Create a new avatar and equip its starting weapon.</summary>
    public async ValueTask<AvatarDataInfo?> CreateAvatar(int avatarId, bool addToCurrentTeam = true)
    {
        if (!GameData.AvatarData.TryGetValue(avatarId, out var avatarExcel))
            return null;

        if (HasAvatar(avatarId))
            return null;

        var now = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

        var avatar = new AvatarDataInfo
        {
            AvatarId = avatarExcel.Id,
            SkillDepotId = avatarExcel.SkillDepotId,
            Guid = NextGuid(),
            WeaponId = avatarExcel.InitialWeapon,
            BornTime = now,
            WearingFlycloakId = GameConstants.DEFAULT_FLYCLOAK_ID
        };

        avatar.InitDefaultProps(avatarExcel);
        Avatars.Add(avatar);
        Save();

        EquipStartingWeapon(avatar, avatarExcel); // EquipItem already calls RecalcStats

        if (Player.HasSentLoginPackets)
        {
            if (addToCurrentTeam && Player.TeamManager != null)
                Player.TeamManager.AddAvatarToCurrentTeam(avatar.Guid);
        }

        return avatar;
    }

    private void EquipStartingWeapon(AvatarDataInfo avatar, AvatarDataExcel excel,
        int weaponLevel = 0, int refinement = 0)
    {
        if (excel.InitialWeapon <= 0 || !GameData.ItemData.TryGetValue((int)excel.InitialWeapon, out _))
            return;

        int level = weaponLevel > 0 ? weaponLevel : GameConstants.DEFAULT_WEAPON_LEVEL;
        int refine = refinement > 0 ? refinement : GameConstants.DEFAULT_WEAPON_REFINEMENT;
        int promoteLevel = ItemData.GetMinPromoteLevel(level);

        var weapon = new ItemData
        {
            ItemId = (int)excel.InitialWeapon,
            Count = 1,
            Level = level,
            PromoteLevel = promoteLevel,
            Refinement = refine
        };
        weapon.InitWeaponAffixes();

        Player.InventoryManager.AddItem(weapon);
        if (weapon.Guid > 0)
            Player.InventoryManager.EquipItem(avatar.Guid, weapon.Guid);
    }

    #endregion

    #region Level & Promote

    public async Task UpgradeAvatar(ulong avatarGuid, List<ItemParam> itemParamList)
    {
        var avatar = GetAvatarByGuid(avatarGuid);
        if (avatar == null) return;

        int promoteId = avatar.AvatarExcel?.AvatarPromoteId ?? 0;
        int promoteLevel = (int)avatar.PromoteLevel;
        int key = (promoteId << 8) + promoteLevel;

        if (!GameData.AvatarPromoteData.TryGetValue(key, out var promoteData))
            return;

        int maxLevel = promoteData.UnlockMaxLevel;
        if (avatar.Level >= maxLevel) return;

        // Calculate total EXP from submitted items
        int expGain = 0;
        var costItems = new List<ItemParamData>();
        foreach (var itemParam in itemParamList)
        {
            var itemData = GameData.ItemData.GetValueOrDefault((int)itemParam.ItemId);
            if (itemData?.ItemUseActions == null) continue;

            foreach (var action in itemData.ItemUseActions)
            {
                if (action.UseOp != "ITEM_USE_ADD_EXP") continue;
                expGain += (int)(action.Param * itemParam.Count);
                costItems.Add(new ItemParamData((int)itemParam.ItemId, (int)itemParam.Count));
            }
        }
        if (expGain <= 0) return;

        // Mora cost = total EXP / 5
        int moraCost = expGain / 5;
        if (moraCost > 0)
            costItems.Add(new ItemParamData(202, moraCost));

        if (!Player.InventoryManager.PayItems(
                costItems.Select(c => new ItemParam { ItemId = (uint)c.Id, Count = (uint)c.Count }).ToList()))
            return;

        int level = (int)avatar.Level;
        int oldLevel = level;
        int exp = (int)avatar.Exp;
        int reqExp = GetAvatarLevelExpRequired(level);

        // Level-up loop
        while (expGain > 0 && reqExp > 0 && level < maxLevel)
        {
            int toGain = Math.Min(expGain, reqExp - exp);
            exp += toGain;
            expGain -= toGain;
            if (exp >= reqExp)
            {
                exp = 0;
                level++;
                reqExp = GetAvatarLevelExpRequired(level);
            }
        }

        avatar.Level = (uint)level;
        avatar.Exp = (uint)exp;
        avatar.SetProp(PlayerProp.PROP_LEVEL, (uint)level);
        avatar.SetProp(PlayerProp.PROP_EXP, (uint)exp);

        // Recalc with equipped weapon
        var weapon = Player.InventoryManager.Items.Values
            .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId && i.ItemType == Enums.Item.ItemType.ITEM_WEAPON);
        avatar.RecalcStats(weapon);
        Save();

        await Player.SendPacket(new PacketAvatarUpgradeRsp(avatar, oldLevel, level));
        await Player.SendPacket(new PacketAvatarPropNotify(avatar));
    }

    public async Task PromoteAvatar(ulong avatarGuid)
    {
        var avatar = GetAvatarByGuid(avatarGuid);
        if (avatar == null || avatar.AvatarExcel == null) return;

        int nextPromoteLevel = (int)avatar.PromoteLevel + 1;
        int promoteId = avatar.AvatarExcel.AvatarPromoteId;

        int curKey = (promoteId << 8) + (int)avatar.PromoteLevel;
        int nextKey = (promoteId << 8) + nextPromoteLevel;

        if (!GameData.AvatarPromoteData.TryGetValue(curKey, out var curData) ||
            !GameData.AvatarPromoteData.TryGetValue(nextKey, out var nextData))
            return;

        // Must be at current tier's max level to promote
        if (avatar.Level != curData.UnlockMaxLevel) return;

        // Pay costs
        var costs = new List<ItemParamData>(nextData.CostItems);
        if (nextData.CoinCost > 0)
            costs.Add(new ItemParamData(202, nextData.CoinCost));

        if (!Player.InventoryManager.PayItems(
                costs.Select(c => new ItemParam { ItemId = (uint)c.Id, Count = (uint)c.Count }).ToList()))
            return;

        avatar.PromoteLevel = (uint)nextPromoteLevel;
        avatar.SetProp(PlayerProp.PROP_BREAK_LEVEL, (uint)nextPromoteLevel);

        // Unlock new proud skills unlocked by this ascension
        UnlockPromoteProudSkills(avatar, nextPromoteLevel);

        var weapon = Player.InventoryManager.Items.Values
            .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId && i.ItemType == Enums.Item.ItemType.ITEM_WEAPON);
        avatar.RecalcStats(weapon);
        Save();

        await Player.SendPacket(new PacketAvatarPromoteRsp(avatar));
        await Player.SendPacket(new PacketAvatarPropNotify(avatar));
    }

    private void UnlockPromoteProudSkills(AvatarDataInfo avatar, int promoteLevel)
    {
        if (!GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depotData))
            return;

        foreach (var open in depotData.InherentProudSkillOpens)
        {
            if (open.NeedAvatarPromoteLevel != promoteLevel) continue;
            int proudSkillId = (int)(open.ProudSkillGroupId * 100) + 1;
            if (!GameData.ProudSkillData.ContainsKey(proudSkillId)) continue;

            if (!avatar.ProudSkillList.Contains((uint)proudSkillId))
            {
                avatar.ProudSkillList.Add((uint)proudSkillId);
                _ = Player.SendPacket(new PacketProudSkillChangeNotify(avatar));
            }
        }
    }

    private static int GetAvatarLevelExpRequired(int level)
    {
        if (GameData.AvatarLevelData.TryGetValue(level, out var data))
            return data.Exp;
        return level * 100;
    }

    /// <summary>Set avatar to max level/promote/skills/constellation, max weapon, and recalc.</summary>
    public void MaxOutAvatar(AvatarDataInfo avatar, int maxLevel = 90, int maxPromote = 6, int maxSkill = 10)
    {
        int maxConstellation = 6;

        avatar.Level = (uint)maxLevel;
        avatar.PromoteLevel = (uint)maxPromote;
        avatar.Exp = 0;
        avatar.SetProp(PlayerProp.PROP_LEVEL, maxLevel);
        avatar.SetProp(PlayerProp.PROP_EXP, 0);
        avatar.SetProp(PlayerProp.PROP_BREAK_LEVEL, maxPromote);

        if (GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depot))
        {
            foreach (var skillId in depot.GetSkillsAndEnergySkill())
                avatar.SkillLevelMap[skillId] = (uint)maxSkill;

            // Rebuild proud skill list for all ascension levels
            avatar.ProudSkillList.Clear();
            foreach (var open in depot.InherentProudSkillOpens)
            {
                if (open.ProudSkillGroupId <= 0) continue;
                if (open.NeedAvatarPromoteLevel > maxPromote) continue;
                int proudSkillId = (int)(open.ProudSkillGroupId * 100) + 1;
                if (GameData.ProudSkillData.ContainsKey(proudSkillId))
                    avatar.ProudSkillList.Add((uint)proudSkillId);
            }
        }

        ForceConstellationLevel(avatar, maxConstellation);
        avatar.CoreProudSkillLevel = (uint)maxConstellation;

        // Max out equipped weapon
        MaxOutEquippedWeapon(avatar);
    }

    /// <summary>Upgrade the avatar's equipped weapon to max level/refinement.</summary>
    private void MaxOutEquippedWeapon(AvatarDataInfo avatar)
    {
        var weapon = GetEquippedWeapon(avatar);
        if (weapon == null) return;

        var weaponData = weapon.ItemDataExcel;
        if (weaponData == null) return;

        weapon.Level = 90;
        weapon.PromoteLevel = ItemData.GetMinPromoteLevel(90);
        weapon.Refinement = 4; // hk4e: refinement is 0-indexed (0..4 = R1..R5)

        if (weapon.Affixes.Count == 0)
            weapon.InitWeaponAffixes();

        Player.InventoryManager.Save();
        avatar.RecalcStats(weapon);
    }

    #endregion

    #region Skill Upgrade

    public async Task UpgradeSkill(ulong avatarGuid, int skillId)
    {
        var avatar = GetAvatarByGuid(avatarGuid);
        if (avatar == null) return;

        if (!GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depotData))
            return;

        var validSkills = new HashSet<uint>(depotData.Skills);
        if (depotData.EnergySkill > 0)
            validSkills.Add(depotData.EnergySkill);

        if (!validSkills.Contains((uint)skillId)) return;

        if (!GameData.AvatarSkillData.TryGetValue(skillId, out var skillData))
            return;

        var oldLevel = avatar.SkillLevelMap.GetValueOrDefault((uint)skillId, 1u);
        var newLevel = oldLevel + 1;

        var proudSkillGroupId = skillData.ProudSkillGroupId;
        if (proudSkillGroupId <= 0) return;

        // Find the next proud skill entry for this group and level
        var nextProudSkill = GameData.ProudSkillData.Values
            .FirstOrDefault(p => p.ProudSkillGroupId == proudSkillGroupId && p.Level == (int)newLevel);
        if (nextProudSkill == null) return;

        if (avatar.PromoteLevel < nextProudSkill.BreakLevel) return;

        // Pay mora cost
        if (nextProudSkill.CoinCost > 0)
        {
            if (!Player.InventoryManager.PayItem(202, nextProudSkill.CoinCost))
                return;
        }

        // Pay material costs
        if (nextProudSkill.CostItems.Count > 0)
        {
            var itemParams = nextProudSkill.CostItems
                .Select(c => new ItemParam { ItemId = (uint)c.Id, Count = (uint)c.Count })
                .ToList();
            if (!Player.InventoryManager.PayItems(itemParams))
                return;
        }

        avatar.SkillLevelMap[(uint)skillId] = (uint)newLevel;
        Save();

        await Player.SendPacket(new PacketAvatarSkillUpgradeRsp(avatar, skillId, (int)oldLevel, (int)newLevel));
        await Player.SendPacket(new PacketAvatarSkillChangeNotify(avatar, skillId, (int)oldLevel, (int)newLevel));

        // Check for inherent/special proud skill unlock
        CheckAndUnlockProudSkills(avatar, depotData);
    }

    private void CheckAndUnlockProudSkills(AvatarDataInfo avatar, AvatarSkillDepotDataExcel depotData)
    {
        var unlocked = false;

        foreach (var open in depotData.InherentProudSkillOpens)
        {
            int proudSkillId = (int)(open.ProudSkillGroupId * 100) + 1;
            if (avatar.PromoteLevel >= open.NeedAvatarPromoteLevel
                && GameData.ProudSkillData.ContainsKey(proudSkillId)
                && avatar.ProudSkillList.All(id => id != (uint)proudSkillId))
            {
                avatar.ProudSkillList.Add((uint)proudSkillId);
                unlocked = true;
            }
        }

        if (unlocked)
        {
            Save();
            _ = Player.SendPacket(new PacketProudSkillChangeNotify(avatar));
        }
    }

    #endregion

    #region Constellation / Talent Unlock


    public async Task UnlockConstellation(AvatarDataInfo avatar, int talentId)
    {
        if (avatar == null) return;

        if (!GameData.AvatarTalentData.TryGetValue(talentId, out var talentData))
            return;

        if (avatar.TalentIdList.Contains((uint)talentId)) return;

        // Check prev talent chain
        if (talentData.PrevTalent > 0 && !avatar.TalentIdList.Contains((uint)talentData.PrevTalent))
            return;

        // Pay main cost item
        if (talentData.MainCostItemId > 0 && talentData.MainCostItemCount > 0)
        {
            if (!Player.InventoryManager.PayItem(talentData.MainCostItemId, talentData.MainCostItemCount))
                return;
        }

        avatar.TalentIdList.Add((uint)talentId);
        avatar.CoreProudSkillLevel = (uint)avatar.TalentIdList.Count;

        // Apply talent stat bonuses
        foreach (var prop in talentData.AddProps)
        {
            if (FightProp.NameMap.TryGetValue(prop.PropType, out var propId))
            {
                var current = avatar.GetFightProp(propId);
                avatar.SetFightProp(propId, current + prop.Value);
            }
        }

        // Apply +3 extra levels to proud skills from skill depot talents
        if (GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depotData))
        {
            var talentIndex = depotData.Talents.IndexOf((uint)talentId);
            if (talentIndex >= 0)
            {
                foreach (var skillId in depotData.GetSkillsAndEnergySkill())
                {
                    if (GameData.AvatarSkillData.TryGetValue((int)skillId, out var skillData)
                        && skillData.ProudSkillGroupId > 0)
                    {
                        var currentExtra = avatar.ProudSkillExtraLevelMap.GetValueOrDefault((uint)skillData.ProudSkillGroupId, 0u);
                        avatar.ProudSkillExtraLevelMap[(uint)skillData.ProudSkillGroupId] = currentExtra + 3;
                    }
                }

                _ = Player.SendPacket(new PacketProudSkillExtraLevelNotify(avatar, talentIndex));
            }
        }

        var weapon = Player.InventoryManager.Items.Values
            .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId && i.ItemType == Enums.Item.ItemType.ITEM_WEAPON);
        avatar.RecalcStats(weapon);
        Save();

        await Player.SendPacket(new PacketUnlockAvatarTalentRsp(avatar, talentId));
        await Player.SendPacket(new PacketAvatarUnlockTalentNotify(avatar, talentId));
    }
    
    public void ForceConstellationLevel(AvatarDataInfo avatar, int level)
    {
        if (level > 6) return;

        if (level < 0)
        {
            avatar.TalentIdList.Clear();
            avatar.CoreProudSkillLevel = 0;
            avatar.RecalcStats(GetEquippedWeapon(avatar));
            Save();
            return;
        }

        if (!GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depot))
            return;

        avatar.TalentIdList.Clear();
        avatar.CoreProudSkillLevel = 0;

        for (int i = 0; i < level && i < depot.Talents.Count; i++)
        {
            var talentId = (int)depot.Talents[i];
            if (!GameData.AvatarTalentData.TryGetValue(talentId, out var talentData))
                continue;

            avatar.TalentIdList.Add((uint)talentId);

            foreach (var prop in talentData.AddProps)
            {
                if (FightProp.NameMap.TryGetValue(prop.PropType, out var propId))
                    avatar.SetFightProp(propId, avatar.GetFightProp(propId) + prop.Value);
            }

            // Apply +3 extra levels to proud skills from depot talents
            var talentIndex = depot.Talents.IndexOf((uint)talentId);
            if (talentIndex >= 0)
            {
                foreach (var skillId in depot.GetSkillsAndEnergySkill())
                {
                    if (GameData.AvatarSkillData.TryGetValue((int)skillId, out var skillData)
                        && skillData.ProudSkillGroupId > 0)
                    {
                        var currentExtra = avatar.ProudSkillExtraLevelMap.GetValueOrDefault((uint)skillData.ProudSkillGroupId, 0u);
                        avatar.ProudSkillExtraLevelMap[(uint)skillData.ProudSkillGroupId] = currentExtra + 3;
                    }
                }
            }
        }

        avatar.CoreProudSkillLevel = (uint)avatar.TalentIdList.Count;
        avatar.RecalcStats(GetEquippedWeapon(avatar));
        Save();
    }

    private ItemData? GetEquippedWeapon(AvatarDataInfo avatar)
        => Player.InventoryManager.Items.Values
            .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId
                && i.ItemType == Enums.Item.ItemType.ITEM_WEAPON);

    #endregion
}
