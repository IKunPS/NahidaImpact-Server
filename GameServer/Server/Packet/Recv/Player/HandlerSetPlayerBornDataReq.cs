using NahidaImpact.Data;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

/// <summary>
/// Handles the player's character selection after the opening cutscene.
/// Only reached when Questing.EnabledBornQuest = true.
/// Mirrors Java HandlerSetPlayerBornDataReq.
/// </summary>
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
        int startingSkillDepot;
        int headImage;

        if (avatarId == GameConstants.MAIN_CHARACTER_MALE)
        {
            startingSkillDepot = 504;
            headImage = 1;
        }
        else if (avatarId == GameConstants.MAIN_CHARACTER_FEMALE)
        {
            startingSkillDepot = 704;
            headImage = 2;
        }
        else
        {
            return;
        }

        // Validate avatar data exists
        if (!GameData.AvatarData.ContainsKey(avatarId))
        {
            Logger.Error("No avatar data found! Check your ExcelBinOutput folder.");
            connection.Stop();
            return;
        }

        // Set nickname from client
        player.Data.Name = req.NickName;
        player.Profile.Nickname = req.NickName;

        // Create the chosen main character (only if no avatars yet)
        if (player.AvatarManager.AvatarCount == 0)
        {
            var mainCharacter = await player.AddAvatar(avatarId, false);
            if (mainCharacter == null) return;

            mainCharacter.SkillDepotId = (uint)startingSkillDepot;
            player.MainCharacterId = avatarId;

            // Manually add to team 1 (mirrors Java)
            player.TeamManager.GetCurrentSinglePlayerTeamInfo().AvatarGuidList.Add(mainCharacter.Guid);
            player.TeamManager.Save();
        }
        else
        {
            return;
        }

        // Complete login
        await player.OnLogin();

        // Trigger born quests (mirrors Java player.onPlayerBorn())
        // TODO: player.QuestManager.OnPlayerBorn();

        // Send born response
        await player.SendPacket(new BasePacket(CmdIds.SetPlayerBornDataRsp));

        // TODO: Send welcome mail (like Java version)
    }
}