using NahidaImpact.Data;
using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Inventory;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Game.Inventory;

public class WeaponManager(PlayerInstance player) : BasePlayerManager(player)
{
    private InventoryManager Inventory => Player.InventoryManager;

    private const int MaxRefinement = 5;
    private const int MaxPromoteLevel = 6;
    private const int WeaponExpItemId = 104; // Enhancement ore base id

    public WeaponUpgradeResult UpgradeWeapon(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        var weapon = Inventory.GetItemByGuid(targetGuid);
        if (weapon == null || weapon.ItemType != Enums.Item.ItemType.ITEM_WEAPON)
            return WeaponUpgradeResult.Fail(Retcode.RetItemNotExist);

        var itemData = weapon.GetItemData();
        if (itemData == null)
            return WeaponUpgradeResult.Fail(Retcode.RetItemNotExist);

        int maxLevel = GetMaxLevelByPromote(weapon.PromoteLevel);
        if (weapon.Level >= maxLevel)
            return WeaponUpgradeResult.Fail(Retcode.RetEquipLevelHigher);

        int oldLevel = weapon.Level;
        int totalExp = CalculateTotalExp(foodGuids);

        if (!Inventory.PayItems(itemParams))
            return WeaponUpgradeResult.Fail(Retcode.RetMcoinNotEnough);

        foreach (var guid in foodGuids)
        {
            var fodder = Inventory.GetItemByGuid(guid);
            if (fodder != null)
                Inventory.RemoveItem(fodder, fodder.Count);
        }

        totalExp += weapon.TotalExp;
        int newLevel = oldLevel;
        int remainingExp = totalExp;

        while (newLevel < maxLevel && remainingExp > 0)
        {
            int needExp = GetExpRequired(newLevel + 1);
            if (remainingExp < needExp) break;
            remainingExp -= needExp;
            newLevel++;
        }

        weapon.Level = newLevel;
        weapon.TotalExp = totalExp;
        weapon.Exp = remainingExp;
        Inventory.Save();

        RecalcEquippedAvatar(weapon);

        var returnItems = CalculateReturnItems(weapon, foodGuids);
        return WeaponUpgradeResult.Success(oldLevel, newLevel, returnItems);
    }

    public WeaponPromoteResult PromoteWeapon(ulong targetGuid)
    {
        var weapon = Inventory.GetItemByGuid(targetGuid);
        if (weapon == null || weapon.ItemType != Enums.Item.ItemType.ITEM_WEAPON)
            return WeaponPromoteResult.Fail(Retcode.RetItemNotExist);

        var itemData = weapon.GetItemData();
        if (itemData == null)
            return WeaponPromoteResult.Fail(Retcode.RetItemNotExist);

        if (weapon.PromoteLevel >= MaxPromoteLevel)
            return WeaponPromoteResult.Fail(Retcode.RetAwakenLevelMax);

        int promoteId = (int)itemData.WeaponPromoteId;
        int nextPromote = weapon.PromoteLevel + 1;
        int key = (promoteId << 8) + nextPromote;

        if (!GameData.WeaponPromoteData.TryGetValue(key, out var promoteData))
            return WeaponPromoteResult.Fail(Retcode.RetSvrError);

        int maxLevel = GetMaxLevelByPromote(weapon.PromoteLevel);
        if (weapon.Level < maxLevel)
            return WeaponPromoteResult.Fail(Retcode.RetEquipLevelHigher);

        if (promoteData.CoinCost > 0 && !Inventory.PayItem(202, promoteData.CoinCost))
            return WeaponPromoteResult.Fail(Retcode.RetMcoinNotEnough);

        if (promoteData.CostItems.Count > 0)
        {
            var itemParams = promoteData.CostItems
                .Select(c => new ItemParam { ItemId = (uint)c.Id, Count = (uint)c.Count })
                .ToList();
            if (!Inventory.PayItems(itemParams))
                return WeaponPromoteResult.Fail(Retcode.RetItemNotExist);
        }

        int oldPromoteLevel = weapon.PromoteLevel;
        weapon.PromoteLevel = nextPromote;
        Inventory.Save();

        RecalcEquippedAvatar(weapon);

        return WeaponPromoteResult.Success(oldPromoteLevel, nextPromote);
    }

    public WeaponAwakenResult AwakenWeapon(ulong targetGuid, List<ulong> itemGuids)
    {
        var weapon = Inventory.GetItemByGuid(targetGuid);
        if (weapon == null || weapon.ItemType != Enums.Item.ItemType.ITEM_WEAPON)
            return WeaponAwakenResult.Fail(Retcode.RetItemNotExist);

        var itemData = weapon.GetItemData();
        if (itemData == null)
            return WeaponAwakenResult.Fail(Retcode.RetItemNotExist);

        if (weapon.Refinement >= MaxRefinement)
            return WeaponAwakenResult.Fail(Retcode.RetAwakenLevelMax);

        int awakenMaterialId = (int)itemData.AwakenMaterial;
        if (awakenMaterialId <= 0)
            return WeaponAwakenResult.Fail(Retcode.RetSvrError);

        int consumeCount = 0;
        foreach (var guid in itemGuids)
        {
            var fodder = Inventory.GetItemByGuid(guid);
            if (fodder == null) continue;
            if (fodder.ItemId == awakenMaterialId)
            {
                Inventory.RemoveItem(guid);
                consumeCount++;
            }
        }

        if (consumeCount <= 0)
            return WeaponAwakenResult.Fail(Retcode.RetItemNotExist);

        var oldAffixMap = new Dictionary<uint, uint>();
        if (weapon.Affixes.Count > 0)
        {
            foreach (var affix in weapon.Affixes)
                oldAffixMap[(uint)affix] = (uint)weapon.Refinement;
        }

        int oldRefinement = weapon.Refinement;
        weapon.Refinement = Math.Min(weapon.Refinement + 1, MaxRefinement);
        Inventory.Save();

        RecalcEquippedAvatar(weapon);

        var curAffixMap = new Dictionary<uint, uint>();
        if (weapon.Affixes.Count > 0)
        {
            foreach (var affix in weapon.Affixes)
                curAffixMap[(uint)affix] = (uint)weapon.Refinement;
        }

        return WeaponAwakenResult.Success(oldAffixMap, curAffixMap, weapon.Refinement);
    }

    public CalcWeaponUpgradeResult CalcWeaponUpgradeReturnItems(ulong targetGuid, List<ulong> foodGuids, List<ItemParam> itemParams)
    {
        var weapon = Inventory.GetItemByGuid(targetGuid);
        if (weapon == null || weapon.ItemType != Enums.Item.ItemType.ITEM_WEAPON)
            return CalcWeaponUpgradeResult.Fail(Retcode.RetItemNotExist);

        var returnItems = CalculateReturnItems(weapon, foodGuids);
        return CalcWeaponUpgradeResult.Success(returnItems);
    }

    private int CalculateTotalExp(List<ulong> foodGuids)
    {
        int total = 0;
        foreach (var guid in foodGuids)
        {
            var item = Inventory.GetItemByGuid(guid);
            if (item == null) continue;
            var data = item.GetItemData();
            if (data == null) continue;
            total += (int)(data.WeaponBaseExp * item.Count);
        }
        return total;
    }

    private List<ItemParam> CalculateReturnItems(ItemData weapon, List<ulong> foodGuids)
    {
        var returnItems = new List<ItemParam>();
        var itemData = weapon.GetItemData();
        if (itemData == null) return returnItems;

        int maxLevel = GetMaxLevelByPromote(weapon.PromoteLevel);
        int totalExp = weapon.TotalExp + CalculateTotalExp(foodGuids);
        int currentLevel = weapon.Level;
        int expRemaining = totalExp;

        while (currentLevel < maxLevel && expRemaining > 0)
        {
            int needExp = GetExpRequired(currentLevel + 1);
            if (expRemaining < needExp) break;
            expRemaining -= needExp;
            currentLevel++;
        }

        while (currentLevel >= maxLevel && expRemaining > 0)
        {
            int refundExp = expRemaining;
            int refundCount = refundExp / (int)itemData.WeaponBaseExp;
            if (refundCount > 0)
            {
                returnItems.Add(new ItemParam { ItemId = (uint)weapon.ItemId, Count = (uint)refundCount });
            }
            break;
        }

        return returnItems;
    }

    public static int GetExpRequired(int level)
    {
        if (GameData.WeaponLevelData.TryGetValue(level, out var levelData) && levelData.RequiredExps.Count > 0)
            return levelData.RequiredExps[0];
        return level * 100; // Fallback
    }

    public static int GetMaxLevelByPromote(int promoteLevel)
    {
        return promoteLevel switch
        {
            0 => 20,
            1 => 40,
            2 => 50,
            3 => 60,
            4 => 70,
            5 => 80,
            6 => 90,
            _ => 90
        };
    }

    /// <summary>Recalc stats for the avatar that has this weapon equipped.</summary>
    private void RecalcEquippedAvatar(ItemData weapon)
    {
        if (weapon.EquipCharacter <= 0) return;
        var avatar = Player.AvatarManager.GetAvatarById((uint)weapon.EquipCharacter);
        avatar?.RecalcStats(weapon);
    }

    public static int GetMinPromoteLevel(int level)
    {
        if (level > 80) return 6;
        if (level > 70) return 5;
        if (level > 60) return 4;
        if (level > 50) return 3;
        if (level > 40) return 2;
        if (level > 20) return 1;
        return 0;
    }
}

public class WeaponUpgradeResult
{
    public bool IsSuccess { get; set; }
    public int Retcode { get; set; }
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public List<ItemParam> ReturnItems { get; set; } = [];

    public static WeaponUpgradeResult Fail(Retcode retcode) => new() { IsSuccess = false, Retcode = (int)retcode };
    public static WeaponUpgradeResult Success(int oldLevel, int newLevel, List<ItemParam> returnItems) =>
        new() { IsSuccess = true, OldLevel = oldLevel, NewLevel = newLevel, ReturnItems = returnItems };
}

public class WeaponPromoteResult
{
    public bool IsSuccess { get; set; }
    public int Retcode { get; set; }
    public int OldPromoteLevel { get; set; }
    public int NewPromoteLevel { get; set; }

    public static WeaponPromoteResult Fail(Retcode retcode) => new() { IsSuccess = false, Retcode = (int)retcode };
    public static WeaponPromoteResult Success(int oldLevel, int newLevel) =>
        new() { IsSuccess = true, OldPromoteLevel = oldLevel, NewPromoteLevel = newLevel };
}

public class WeaponAwakenResult
{
    public bool IsSuccess { get; set; }
    public int Retcode { get; set; }
    public Dictionary<uint, uint> OldAffixMap { get; set; } = [];
    public Dictionary<uint, uint> CurAffixMap { get; set; } = [];
    public int AwakenLevel { get; set; }

    public static WeaponAwakenResult Fail(Retcode retcode) => new() { IsSuccess = false, Retcode = (int)retcode };
    public static WeaponAwakenResult Success(Dictionary<uint, uint> oldAffix, Dictionary<uint, uint> curAffix, int level) =>
        new() { IsSuccess = true, OldAffixMap = oldAffix, CurAffixMap = curAffix, AwakenLevel = level };
}

public class CalcWeaponUpgradeResult
{
    public bool IsSuccess { get; set; }
    public int Retcode { get; set; }
    public List<ItemParam> ReturnItems { get; set; } = [];

    public static CalcWeaponUpgradeResult Fail(Retcode retcode) => new() { IsSuccess = false, Retcode = (int)retcode };
    public static CalcWeaponUpgradeResult Success(List<ItemParam> returnItems) =>
        new() { IsSuccess = true, ReturnItems = returnItems };
}