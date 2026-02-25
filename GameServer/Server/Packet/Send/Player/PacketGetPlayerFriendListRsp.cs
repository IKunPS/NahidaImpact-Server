using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.KcpSharp;
using NahidaImpact.Proto;
using NahidaImpact.Util;

namespace NahidaImpact.GameServer.Server.Packet.Send.Player;

public class PacketGetPlayerFriendListRsp : BasePacket
{
    public PacketGetPlayerFriendListRsp(PlayerInstance player) : base(CmdIds.GetPlayerFriendListRsp)
    {
        var proto = new GetPlayerFriendListRsp();

        // Add server profile as a friend
        var serverAccount = ConfigManager.Config.ServerProfile;
        var serverFriend = new FriendBrief
        {
            Uid = (uint)serverAccount.Uid,
            Nickname = serverAccount.NickName,
            Level = serverAccount.AdventureRank,
            ProfilePicture = new ProfilePicture { AvatarId = (uint)serverAccount.AvatarId },
            WorldLevel = (uint)serverAccount.WorldLevel,
            Signature = serverAccount.Signature,
            LastActiveTime = (uint)DateTimeOffset.Now.ToUnixTimeSeconds(),
            NameCardId = (uint)serverAccount.NameCardId,
            OnlineState = FriendOnlineState.FriendOnline,
            Param = 1,
            IsGameSource = true,
            PlatformType = PlatformType.Pc
        };
        proto.FriendList.Add(serverFriend);

        // Add player's friends from SocialManager
        if (player.SocialManager != null)
        {
            proto.FriendList.AddRange(player.SocialManager.GetFriendBriefList());
            proto.AskFriendList.AddRange(player.SocialManager.GetPendingFriendBriefList());
        }

        SetData(proto);
    }
}
