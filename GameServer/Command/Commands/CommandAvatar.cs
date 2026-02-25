using NahidaImpact.Enums.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Avatar;
using NahidaImpact.Internationalization;

namespace NahidaImpact.GameServer.Command.Commands;

[CommandInfo("avatar", "Game.Command.Avatar.Desc", "Game.Command.Avatar.Usage", ["a", "give"], [PermEnum.Admin, PermEnum.Support])]
public class CommandAvatar : ICommands
{
    [CommandDefault]
    public static async ValueTask GiveAvatar(CommandArg arg)
    {
        if (!await arg.CheckArgCnt(1, 2)) return;
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player;
        if (player?.AvatarManager == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotInit"));
            return;
        }

        var avatarId = arg.GetInt(0);
        var level = arg.Args.Count >= 2 ? arg.GetInt(1) : 90;

        if (avatarId == 0)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Avatar.InvalidId"));
            return;
        }

        if (level < 1 || level > 90)
        {
            level = 90;
        }

        var result = await player.AvatarManager.AddAvatar(avatarId, level);
        if (result == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Avatar.AddFailed", avatarId.ToString()));
            return;
        }

        // Send avatar data update to player
        await player.SendPacket(new PacketAvatarDataNotify(player, player.Avatars!));

        await arg.SendMsg(I18NManager.Translate("Game.Command.Avatar.AddSuccess", result.Id.ToString(), level.ToString(), player.Uid.ToString()));
    }

    [CommandMethod("all")]
    public static async ValueTask GiveAllAvatars(CommandArg arg)
    {
        if (!await arg.CheckOnlineTarget()) return;

        var player = arg.Target!.Player;
        if (player?.AvatarManager == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotInit"));
            return;
        }

        var level = arg.Args.Count >= 1 ? arg.GetInt(0) : 90;
        if (level < 1 || level > 90)
        {
            level = 90;
        }

        int count = 0;
        // Avatar IDs range: 10000002 - 10000100+ (common avatar range)
        for (int avatarId = 10000002; avatarId <= 10000100; avatarId++)
        {
            var result = await player.AvatarManager.AddAvatar(avatarId, level);
            if (result != null) count++;
        }

        // Also try some newer avatars
        for (int avatarId = 10000100; avatarId <= 10000200; avatarId++)
        {
            var result = await player.AvatarManager.AddAvatar(avatarId, level);
            if (result != null) count++;
        }

        // Send avatar data update to player
        await player.SendPacket(new PacketAvatarDataNotify(player, player.Avatars!));

        await arg.SendMsg(I18NManager.Translate("Game.Command.Avatar.AddAllSuccess", count.ToString(), level.ToString(), player.Uid.ToString()));
    }
}
