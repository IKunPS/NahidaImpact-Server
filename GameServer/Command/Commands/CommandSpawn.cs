using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.GameServer.Server.Packet.Send.State;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("spawn", "Game.Command.Spawn.Desc", "Game.Command.Spawn.Usage", [], [PermEnum.Support, PermEnum.Admin])]
public class CommandSpawn : ICommand
{
    [CommandDefault]
    public static async ValueTask Execute(CommandArg arg)
    {
        if (!await arg.CheckTarget()) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player!;
        var scene = player.Scene;
        if (scene == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.NoScene"));
            return;
        }

        if (arg.Args.Count < 2)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.Usage"));
            return;
        }

        var type = arg.Args[0].ToLower();
        if (!int.TryParse(arg.Args[1], out var entityId))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.InvalidEntityId"));
            return;
        }

        int amount = arg.Args.Count >= 3 ? arg.GetInt(2) : 1;
        if (amount <= 0) amount = 1;
        int level = arg.Args.Count >= 4 ? arg.GetInt(3) : 1;
        if (level <= 0) level = 1;

        // Spawn near the player
        var pos = player.Position.Clone();
        pos.X += 3; // Offset a bit in front

        int spawned = 0;
        switch (type)
        {
            case "monster":
            case "mon":
                for (int i = 0; i < amount; i++)
                {
                    var offsetPos = pos.Clone();
                    offsetPos.X += i * 2; // Spread out
                    if (GameData.MonsterData.TryGetValue(entityId, out var monsterData))
                    {
                        var monster = new EntityMonster(scene, monsterData, offsetPos, null, level);
                        scene.AddEntity(monster);
                        await player.SendPacket(new PacketServerGlobalValueChangeNotify(monster.Id, "SGV_MONSTER_SHOUGUN_MITAKENARUKAMI_TRANSFORM", 1));
                        spawned++;
                    }
                }
                break;

            case "gadget":
            case "gad":
                for (int i = 0; i < amount; i++)
                {
                    var offsetPos = pos.Clone();
                    offsetPos.X += i * 2;
                    var gadget = new EntityGadget(scene, entityId, offsetPos, null);
                    scene.AddEntity(gadget);
                    spawned++;
                }
                break;

            default:
                await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.Usage"));
                return;
        }

        await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.Success",
            spawned.ToString(), type, entityId.ToString(), level.ToString(), player.Uid.ToString()));
    }
}