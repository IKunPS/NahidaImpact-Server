using NahidaImpact.Configuration;
using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PlayerLoginReq)]
public class HandlerPlayerLoginReq : Handler
{
    private static readonly Logger Logger = new("HandlerPlayerLoginReq");
    private static readonly Random Rng = new();

    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var player = connection.Player;
        if (player == null)
        {
            connection.Stop();
            return;
        }

        if (player.AvatarManager.AvatarCount == 0)
        {
            if (ConfigManager.Config.GameOptions.Questing.EnabledBornQuest)
            {
                connection.State = SessionStateEnum.PICKING_CHARACTER;
                await connection.SendPacket(new BasePacket(CmdIds.DoSetPlayerBornDataNotify));
                return;
            }

            await AutoCreateMainCharacter(player);
        }
        else
        {
            await player.OnLogin();
        }

        await connection.SendPacket(new PacketPlayerLoginRsp(connection));
    }

    private static async ValueTask AutoCreateMainCharacter(PlayerInstance player)
    {
        bool isMale = Rng.Next(2) == 0;
        int avatarId = isMale ? GameConstants.MAIN_CHARACTER_MALE : GameConstants.MAIN_CHARACTER_FEMALE;

        if (!GameData.AvatarData.ContainsKey(avatarId))
        {
            Logger.Error(I18NManager.Translate("Game.SceneInfo.NoAvatarData"));
            return;
        }

        await player.CompleteFirstLogin(avatarId);
    }
}
