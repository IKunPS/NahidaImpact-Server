using NahidaImpact.Data;
using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Game.Worlds;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("spawn", "Game.Command.Spawn.Desc", "Game.Command.Spawn.Usage", ["drop", "s"], [PermEnum.Support, PermEnum.Admin])]
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

        var args = arg.Args;
        if (args.Count < 1)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.Usage"));
            return;
        }

        // Parse tagged args: lv<level>, x<amount>, state<state>, ai<id>
        int lv = 1, amount = 1, state = -1, ai = -1;
        var positional = new List<string>();
        foreach (var a in args)
        {
            var lower = a.ToLower();
            if (lower.StartsWith("lv") && int.TryParse(a[2..], out var v)) lv = Math.Max(1, v);
            else if (lower.StartsWith('x') && int.TryParse(a[1..], out var c)) amount = Math.Clamp(c, 1, 10);
            else if (lower.StartsWith("state") && int.TryParse(a[5..], out var s)) state = s;
            else if (lower.StartsWith("ai") && int.TryParse(a[2..], out var aiVal)) ai = aiVal;
            else positional.Add(a);
        }

        if (positional.Count < 1 || !int.TryParse(positional[0], out var entityId))
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.InvalidEntityId"));
            return;
        }

        // Position from remaining args: <x> <y> <z>
        var pos = player.Position.Clone();
        if (positional.Count >= 4)
        {
            if (float.TryParse(positional[1], out var px)) pos.X = px;
            if (float.TryParse(positional[2], out var py)) pos.Y = py;
            if (float.TryParse(positional[3], out var pz)) pos.Z = pz;
        }

        var isMonster = GameData.MonsterData.ContainsKey(entityId);
        int spawned = 0;

        if (isMonster)
        {
            var data = GameData.MonsterData[entityId];
            var monsters = new List<EntityMonster>(amount);
            for (int i = 0; i < amount; i++)
            {
                var spread = SpreadPos(pos, i, amount);
                monsters.Add(new EntityMonster(scene, data, spread, null, lv) { AiId = ai });
            }
            scene.AddEntities(monsters);
            spawned = monsters.Count;
        }
        else
        {
            var gadgets = new List<EntityGadget>(amount);
            for (int i = 0; i < amount; i++)
            {
                var spread = SpreadPos(pos, i, amount);
                gadgets.Add(new EntityGadget(scene, entityId, spread) { GadgetState = (uint)state });
            }
            scene.AddEntities(gadgets);
            spawned = gadgets.Count;
        }

        var type = isMonster ? "monster" : "gadget";
        await arg.SendMsg(I18NManager.Translate("Game.Command.Spawn.Success",
            spawned.ToString(), type, entityId.ToString(), lv.ToString(), player.Uid.ToString()));
    }

    private static Position SpreadPos(Position origin, int index, int total)
    {
        if (total <= 1) return origin.Clone();
        var rng = Random.Shared;
        double angle = rng.NextDouble() * Math.PI * 2;
        double radius = Math.Sqrt(total * 0.2 / Math.PI);
        double r = Math.Sqrt(rng.NextDouble() * radius * radius);
        var p = origin.Clone();
        p.X += (float)(r * Math.Cos(angle));
        p.Z += (float)(r * Math.Sin(angle));
        return p;
    }
}