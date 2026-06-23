using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Internationalization;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.SetPlayerBornDataReq)]
public class HandlerSetPlayerBornDataReq : Handler
{
    private static readonly Logger Logger = new("HandlerSetPlayerBornDataReq");

    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetPlayerBornDataReq.Parser.ParseFrom(data);
        var player = connection.Player;
        if (player == null) return;

        int avatarId = (int)req.AvatarId;

        if (avatarId != GameConstants.MAIN_CHARACTER_MALE && avatarId != GameConstants.MAIN_CHARACTER_FEMALE)
            return;

        if (!GameData.AvatarData.ContainsKey(avatarId))
        {
            Logger.Error(I18NManager.Translate("Game.SceneInfo.NoAvatarData"));
            connection.Stop();
            return;
        }

        await player.CompleteFirstLogin(avatarId, req.NickName);
        player.OnPlayerBorn();

        await player.SendPacket(new BasePacket(CmdIds.SetPlayerBornDataRsp));
    }
}
