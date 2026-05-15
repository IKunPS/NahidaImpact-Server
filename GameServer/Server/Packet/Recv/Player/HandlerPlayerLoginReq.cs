using NahidaImpact.Configuration;
using NahidaImpact.Data;
using NahidaImpact.Database.Avatar;
using NahidaImpact.GameServer.Game.Entity;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Recv.Player;

/// <summary>
/// Handles player login. Mirrors Java HandlerPlayerLoginReq.
/// If the player has no avatars, either auto-creates one (skip cutscene)
/// or sends DoSetPlayerBornDataNotify to show the opening cutscene.
/// </summary>
[Opcode(CmdIds.PlayerLoginReq)]
public class HandlerPlayerLoginReq : Handler
{
    private static readonly Logger Logger = new("HandlerPlayerLoginReq");
    private static readonly Random Rng = new();

    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        if (connection.Player == null)
        {
            connection.Stop();
            return;
        }

        var player = connection.Player;

        // Check if a new player with no avatars
        if (player.AvatarManager.AvatarCount == 0)
        {
            if (ConfigManager.Config.GameOptions.Questing.EnabledBornQuest)
            {
                // Show opening cutscene, let the player pick their character
                connection.State = SessionStateEnum.PICKING_CHARACTER;
                await connection.SendPacket(new BasePacket(CmdIds.DoSetPlayerBornDataNotify));
            }
            else
            {
                // Auto-create a random main character, skip cutscene
                int avatarId;
                int startingSkillDepot;

                if (Rng.Next(2) == 0)
                {
                    avatarId = GameConstants.MAIN_CHARACTER_MALE;
                    startingSkillDepot = 504;
                }
                else
                {
                    avatarId = GameConstants.MAIN_CHARACTER_FEMALE;
                    startingSkillDepot = 704;
                }

                // Validate avatar data exists
                if (!GameData.AvatarData.ContainsKey(avatarId))
                {
                    Logger.Error("No avatar data found! Check your ExcelBinOutput folder.");
                    connection.Stop();
                    return;
                }

                // Use AddAvatar (in PlayerInstance) to create the character
                var mainCharacter = await player.AddAvatar(avatarId, false);
                if (mainCharacter == null) return;

                mainCharacter.SkillDepotId = (uint)startingSkillDepot;
                player.MainCharacterId = avatarId;

                // Add to team 1
                player.TeamManager.GetCurrentSinglePlayerTeamInfo().AvatarGuidList.Add(mainCharacter.Guid);
                player.TeamManager.Save();

                // Complete login
                await player.OnLogin();
            }
        }
        else
        {
            // Existing player, login normally
            await player.OnLogin();
        }

        // Final login response
        await connection.SendPacket(new PacketPlayerLoginRsp(connection));
    }
}