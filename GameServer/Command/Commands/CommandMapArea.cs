using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Map;
using NahidaImpact.Internationalization;
using NahidaImpact.Proto;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("maparea", "Game.Command.MapArea.Desc", "Game.Command.MapArea.Usage", ["ma"], [PermEnum.Admin])]
public class CommandMapArea : ICommands
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var args = arg.Args;

        if (args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.Usage"));
            return;
        }

        switch (args[0].ToLower())
        {
            case "give":
                HandleGive(player, args, arg);
                break;
            case "remove":
                HandleRemove(player, args, arg);
                break;
            case "isopen":
                HandleSetOpen(player, args, arg);
                break;
            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.Unknown", args[0]));
                break;
        }
    }

    private static void HandleGive(PlayerInstance player, List<string> args, CommandArg arg)
    {
        if (args.Count < 2)
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.GiveUsage"));
            return;
        }

        var idArg = args[1];
        var isOpen = args.Count > 2 ? bool.Parse(args[2]) : true;
        var changedAreas = new List<MapAreaInfo>();

        if (idArg.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var id in GameData.MapAreaData.Keys)
            {
                player.AddMapArea(id, isOpen);
                changedAreas.Add(player.GetMapAreas()[id]);
            }
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.GiveAll", player.Uid.ToString(), isOpen.ToString()));
        }
        else if (int.TryParse(idArg, out var id))
        {
            if (GameData.MapAreaData.ContainsKey(id))
            {
                player.AddMapArea(id, isOpen);
                changedAreas.Add(player.GetMapAreas()[id]);
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.GiveOne", id.ToString(), player.Uid.ToString(), isOpen.ToString()));
            }
            else
            {
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.InvalidId", idArg));
            }
        }
        else
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.InvalidId", idArg));
        }

        if (changedAreas.Count > 0)
            _ = player.SendPacket(new PacketMapAreaChangeNotify(changedAreas));
    }

    private static void HandleRemove(PlayerInstance player, List<string> args, CommandArg arg)
    {
        if (args.Count < 2)
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.RemoveUsage"));
            return;
        }

        if (int.TryParse(args[1], out var id))
        {
            if (player.GetMapAreas().ContainsKey(id))
            {
                player.RemoveMapArea(id);
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.RemoveSuccess", id.ToString(), player.Uid.ToString()));
                _ = player.SendPacket(new PacketMapAreaChangeNotify([]));
            }
            else
            {
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.NotFound", id.ToString(), player.Uid.ToString()));
            }
        }
        else
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.InvalidId", args[1]));
        }
    }

    private static void HandleSetOpen(PlayerInstance player, List<string> args, CommandArg arg)
    {
        if (args.Count < 3)
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.SetOpenUsage"));
            return;
        }

        if (int.TryParse(args[1], out var id) && bool.TryParse(args[2], out var isOpen))
        {
            if (player.GetMapAreas().ContainsKey(id))
            {
                player.SetMapAreaOpen(id, isOpen);
                var areaInfo = player.GetMapAreas()[id];
                _ = player.SendPacket(new PacketMapAreaChangeNotify([areaInfo]));
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.SetOpenSuccess", id.ToString(), isOpen.ToString(), player.Uid.ToString()));
            }
            else
            {
                _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.NotFound", id.ToString(), player.Uid.ToString()));
            }
        }
        else
        {
            _ = arg.SendMsg(I18NManager.Translate("Game.Command.MapArea.InvalidArgs", args[1], args[2]));
        }
    }
}
