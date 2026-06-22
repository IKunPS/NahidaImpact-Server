using NahidaImpact.Data;
using NahidaImpact.Data.Excel;
using NahidaImpact.Database.Avatar;
using NahidaImpact.Database.Inventory;
using NahidaImpact.Enums.Item;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.GameServer.Server.Packet.Send.Team;
using NahidaImpact.Internationalization;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("give", "Game.Command.Give.Desc", "Game.Command.Give.Usage",
    ["g", "item", "giveitem"], [PermEnum.Admin])]
public class CommandGive : ICommand
{
    private sealed record GiveParams
    {
        public GiveAllType Type = GiveAllType.None;
        public int ItemId;
        public int Level = 1;
        public int Amount = 1;
        public int Refinement = 1;
        public int Constellation = -1;
        public int SkillLevel = 1;
        public int MainPropId = -1;
        public List<int>? AppendPropIdList;
    }

    private enum GiveAllType { None, All, Weapons, Relics, Materials, Furniture, Avatars }

    // Exclude test avatars — range-based, mirrors Java GiveCommand.giveAllAvatars.
    private static bool IsExcludedAvatar(int id) =>
        id < 10000002 || id >= 11000000 || (id is >= 10000900 and <= 10000910);

    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var args = arg.Args;

        if (args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Usage"));
            return;
        }

        var param = new GiveParams();
        var positional = ParseTaggedArgs(args, param);

        if (positional.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Usage"));
            return;
        }

        var sub = positional[0].ToLower();

        // Check give-all subcommands first
        if (await TryHandleGiveAll(player, arg, sub, param))
            return;

        // Handle single item/avatar by ID
        if (!int.TryParse(sub, out var id))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.InvalidItemId"));
            return;
        }
        param.ItemId = id;

        var itemData = GameData.ItemData.GetValueOrDefault(id);
        var isRelic = itemData?.ItemType == ItemType.ITEM_RELIQUARY;

        // Clamp values
        if (param.Amount < 1) param.Amount = 1;
        param.Refinement = Math.Clamp(param.Refinement, 1, 5);

        if (isRelic)
        {
            param.Level = Math.Clamp(param.Level, 0, 20) + 1; // 0..20 -> 1..21

            // Parse relic main prop and substats from remaining positional args
            positional.RemoveAt(0);
            ParseRelicArgs(param, positional);
        }
        else
        {
            param.Level = Math.Clamp(param.Level, 1, 90);
        }

        // Check if it's an avatar (10,000,000-12,000,000 range)
        if (id > 10_000_000 && id < 12_000_000)
        {
            await GiveAvatar(arg, id, param);
            return;
        }

        if (itemData == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.InvalidItemId"));
            return;
        }

        await GiveItem(arg, itemData, param);
    }

    private static List<string> ParseTaggedArgs(List<string> args, GiveParams param)
    {
        var positional = new List<string>();
        foreach (var a in args)
        {
            var lower = a.ToLower();
            if (lower.StartsWith("lv") && int.TryParse(a[2..], out var lv))
                param.Level = lv;
            else if (lower.StartsWith('x') && int.TryParse(a[1..], out var amt))
                param.Amount = amt;
            else if (lower.StartsWith('r') && int.TryParse(a[1..], out var rf))
                param.Refinement = rf;
            else if (lower.StartsWith('c') && int.TryParse(a[1..], out var con))
                param.Constellation = con;
            else if (lower.StartsWith("sl") && int.TryParse(a[2..], out var sk))
                param.SkillLevel = sk;
            else
                positional.Add(a);
        }
        return positional;
    }

    private static void ParseRelicArgs(GiveParams param, List<string> args)
    {
        if (args.Count < 1) return;

        if (int.TryParse(args[0], out var mainProp))
            param.MainPropId = mainProp;

        if (args.Count < 2) return;

        param.AppendPropIdList = new List<int>(args.Count - 1);
        for (int i = 1; i < args.Count; i++)
        {
            var parts = args[i].Split(',');
            if (!int.TryParse(parts[0], out var appendProp))
                continue;

            int n = parts.Length > 1 && int.TryParse(parts[1], out var count)
                ? Math.Clamp(count, 1, 200)
                : 1;

            for (int j = 0; j < n; j++)
                param.AppendPropIdList.Add(appendProp);
        }
    }

    #region Give Single Item

    private static async ValueTask GiveItem(CommandArg arg, ItemDataExcel itemData, GiveParams param)
    {
        var player = arg.Target!.Player!;

        switch (itemData.ItemType)
        {
            case ItemType.ITEM_WEAPON:
                await GiveWeapon(player, arg, itemData, param);
                break;

            case ItemType.ITEM_RELIQUARY:
                await GiveRelic(player, arg, itemData, param);
                break;

            default:
                await GiveMaterial(player, arg, itemData, param);
                break;
        }
    }

    private static async ValueTask GiveWeapon(Game.Player.PlayerInstance player, CommandArg arg,
        ItemDataExcel itemData, GiveParams param)
    {
        int promoteLevel = ItemData.GetMinPromoteLevel(param.Level);
        int totalExp = 0;
        for (int i = 1; i < param.Level; i++)
            totalExp += WeaponManager.GetExpRequired(i);

        var weapon = new ItemData
        {
            ItemId = (int)itemData.Id,
            Count = 1,
            Level = param.Level,
            PromoteLevel = promoteLevel,
            TotalExp = totalExp,
            Refinement = param.Refinement - 1 // 0-indexed
        };
        weapon.InitWeaponAffixes();

        player.InventoryManager.AddItem(weapon, ActionReason.Gm);
        await arg.SendMsg(I18NManager.Translate("Game.Command.Give.GivenWeapon",
            param.Amount.ToString(), itemData.Id.ToString(), param.Level.ToString(),
            param.Refinement.ToString(), player.Uid.ToString()));
    }

    private static async ValueTask GiveRelic(Game.Player.PlayerInstance player, CommandArg arg,
        ItemDataExcel itemData, GiveParams param)
    {
        int rank = (int)itemData.RankLevel;
        int totalExp = 0;
        for (int i = 1; i < param.Level; i++)
            totalExp += i * 100 + rank * 50;

        var relic = new ItemData
        {
            ItemId = (int)itemData.Id,
            Count = 1,
            Level = param.Level,
            TotalExp = totalExp
        };

        if (param.MainPropId > 0)
            relic.MainPropId = param.MainPropId;

        if (param.AppendPropIdList != null)
        {
            relic.AppendPropIdList.Clear();
            relic.AppendPropIdList.AddRange(param.AppendPropIdList);
        }

        player.InventoryManager.AddItem(relic, ActionReason.Gm);
        await arg.SendMsg(I18NManager.Translate("Game.Command.Give.GivenRelic",
            param.Amount.ToString(), itemData.Id.ToString(), (param.Level - 1).ToString(),
            player.Uid.ToString()));
    }

    private static async ValueTask GiveMaterial(Game.Player.PlayerInstance player, CommandArg arg,
        ItemDataExcel itemData, GiveParams param)
    {
        int count = itemData.UseOnGain ? 1 : (int)Math.Min(param.Amount, itemData.StackLimit);
        player.InventoryManager.AddItem((int)itemData.Id, count, ActionReason.Gm);
        await arg.SendMsg(I18NManager.Translate("Game.Command.Give.GivenItem",
            count.ToString(), itemData.Id.ToString(), player.Uid.ToString()));
    }

    private static async ValueTask GiveAvatar(CommandArg arg, int avatarId, GiveParams param)
    {
        var player = arg.Target!.Player!;

        if (!GameData.AvatarData.ContainsKey(avatarId))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.InvalidItemId"));
            return;
        }

        if (player.AvatarManager.HasAvatar(avatarId))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Failed", avatarId.ToString()));
            return;
        }

        var avatar = await player.AvatarManager.CreateAvatar(avatarId, false);
        if (avatar == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Give.Failed", avatarId.ToString()));
            return;
        }

        // Apply level, promote, skills, constellation
        SetupAvatar(player, avatar, param);
        await arg.SendMsg(I18NManager.Translate("Game.Command.Give.GivenAvatar",
            avatarId.ToString(), param.Level.ToString(), player.Uid.ToString()));
    }

    private static void SetupAvatar(Game.Player.PlayerInstance player, AvatarDataInfo avatar, GiveParams param)
    {
        int promoteLevel = ItemData.GetMinPromoteLevel(param.Level);

        avatar.Level = (uint)param.Level;
        avatar.PromoteLevel = (uint)promoteLevel;
        avatar.Exp = 0;
        avatar.SetProp(PlayerProp.PROP_LEVEL, (uint)param.Level);
        avatar.SetProp(PlayerProp.PROP_EXP, 0u);
        avatar.SetProp(PlayerProp.PROP_BREAK_LEVEL, (uint)promoteLevel);

        if (GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depot))
        {
            foreach (var skillId in depot.GetSkillsAndEnergySkill())
                avatar.SkillLevelMap[skillId] = (uint)param.SkillLevel;

            avatar.ProudSkillList.Clear();
            foreach (var open in depot.InherentProudSkillOpens)
            {
                if (open.ProudSkillGroupId <= 0) continue;
                if (open.NeedAvatarPromoteLevel > promoteLevel) continue;
                int proudSkillId = (int)(open.ProudSkillGroupId * 100) + 1;
                if (GameData.ProudSkillData.ContainsKey(proudSkillId))
                    avatar.ProudSkillList.Add((uint)proudSkillId);
            }
        }

        if (param.Constellation >= 0)
            player.AvatarManager.ForceConstellationLevel(avatar, Math.Min(param.Constellation, 6));

        var weapon = player.InventoryManager.Items.Values
            .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId
                && i.ItemType == ItemType.ITEM_WEAPON);
        avatar.RecalcStats(weapon);
        player.AvatarManager.Save();
    }

    #endregion

    #region Give All

    private static async ValueTask<bool> TryHandleGiveAll(Game.Player.PlayerInstance player, CommandArg arg,
        string sub, GiveParams param)
    {
        switch (sub)
        {
            case "all":
                await GiveAllAvatars(player, arg, param);
                await GiveAllByType(player, ItemType.ITEM_WEAPON, param.Amount, param.Level, param.Refinement, arg);
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, 1, 1, arg);
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, param.Amount, 1, 1, arg);
                await GiveAllByType(player, ItemType.ITEM_FURNITURE, param.Amount, 1, 1, arg);
                await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AllGiven", player.Uid.ToString()));
                return true;

            case "weapons":
            case "w":
                await GiveAllByType(player, ItemType.ITEM_WEAPON, param.Amount, param.Level, param.Refinement, arg);
                return true;

            case "relics":
            case "reliquary":
            case "r":
                await GiveAllByType(player, ItemType.ITEM_RELIQUARY, 1, 1, 1, arg);
                return true;

            case "mats":
            case "m":
                await GiveAllByType(player, ItemType.ITEM_MATERIAL, param.Amount, 1, 1, arg);
                return true;

            case "furniture":
            case "f":
                await GiveAllByType(player, ItemType.ITEM_FURNITURE, param.Amount, 1, 1, arg);
                return true;

            case "avatars":
            case "a":
                await GiveAllAvatars(player, arg, param);
                return true;

            default:
                return false;
        }
    }

    private static async ValueTask GiveAllAvatars(Game.Player.PlayerInstance player, CommandArg arg, GiveParams param)
    {
        var added = new List<AvatarDataInfo>();
        int promoteLevel = ItemData.GetMinPromoteLevel(param.Level);

        foreach (var avatarId in GameData.AvatarData.Keys)
        {
            if (IsExcludedAvatar(avatarId)) continue;
            if (player.AvatarManager.HasAvatar(avatarId)) continue;

            try
            {
                var avatar = await player.AvatarManager.CreateAvatar(avatarId, false);
                if (avatar == null) continue;

                avatar.Level = (uint)param.Level;
                avatar.PromoteLevel = (uint)promoteLevel;
                avatar.Exp = 0;
                avatar.SetProp(PlayerProp.PROP_LEVEL, (uint)param.Level);
                avatar.SetProp(PlayerProp.PROP_EXP, 0u);
                avatar.SetProp(PlayerProp.PROP_BREAK_LEVEL, (uint)promoteLevel);

                if (GameData.AvatarSkillDepotData.TryGetValue((int)avatar.SkillDepotId, out var depot))
                {
                    foreach (var skillId in depot.GetSkillsAndEnergySkill())
                        avatar.SkillLevelMap[skillId] = (uint)param.SkillLevel;
                }

                if (param.Constellation >= 0)
                    player.AvatarManager.ForceConstellationLevel(avatar, Math.Min(param.Constellation, 6));

                var weapon = player.InventoryManager.Items.Values
                    .FirstOrDefault(i => i.EquipCharacter == (int)avatar.AvatarId
                        && i.ItemType == ItemType.ITEM_WEAPON);
                avatar.RecalcStats(weapon);
                player.AvatarManager.Save();

                await player.SendPacket(new PacketAvatarAddNotify(avatar, false));
                added.Add(avatar);
            }
            catch { /* skip problematic avatars */ }
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.GiveAll.AvatarsGiven",
            added.Count.ToString(), player.Uid.ToString()));
    }

    private static async ValueTask GiveAllByType(Game.Player.PlayerInstance player, ItemType targetType,
        int amount, int level, int refinement, CommandArg arg)
    {
        var items = new List<ItemData>();
        int created = 0;

        foreach (var (itemId, itemData) in GameData.ItemData)
        {
            if (itemData.ItemType != targetType) continue;
            if (itemData.UseOnGain) continue;

            if (targetType == ItemType.ITEM_WEAPON && !IsValidWeaponId(itemId)) continue;
            if (targetType == ItemType.ITEM_RELIQUARY && !IsValidRelicId(itemId)) continue;
            if ((targetType is ItemType.ITEM_MATERIAL or ItemType.ITEM_FURNITURE) && itemId < 100_000) continue;

            try
            {
                var item = new ItemData { ItemId = itemId };

                if (targetType is ItemType.ITEM_WEAPON or ItemType.ITEM_RELIQUARY)
                {
                    item.Count = 1;
                    item.Level = targetType == ItemType.ITEM_WEAPON ? level : 1;
                    item.PromoteLevel = ItemData.GetMinPromoteLevel(item.Level);

                    if (targetType == ItemType.ITEM_WEAPON)
                    {
                        item.Refinement = refinement - 1;
                        item.InitWeaponAffixes();
                    }
                }
                else
                {
                    item.Count = Math.Min(amount, (int)itemData.StackLimit);
                }

                items.Add(item);
                created++;
            }
            catch { /* skip problematic items */ }
        }

        if (items.Count > 0)
            await player.InventoryManager.AddItemsChunked(items, ActionReason.Gm);

        var key = targetType switch
        {
            ItemType.ITEM_WEAPON => "Game.Command.GiveAll.WeaponsGiven",
            ItemType.ITEM_RELIQUARY => "Game.Command.GiveAll.RelicsGiven",
            ItemType.ITEM_MATERIAL => "Game.Command.GiveAll.MaterialsGiven",
            ItemType.ITEM_FURNITURE => "Game.Command.GiveAll.FurnitureGiven",
            _ => "Game.Command.GiveAll.ItemsGiven"
        };

        await arg.SendMsg(I18NManager.Translate(key,
            created.ToString(), level.ToString(), refinement.ToString(), player.Uid.ToString()));
    }

    private static bool IsValidWeaponId(int id) => id is >= 11100 and <= 16000;
    private static bool IsValidRelicId(int id) => id is >= 20002 and <= 99999;

    #endregion
}