using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Player;

namespace NahidaImpact.GameServer.Game.Friends;

public class SocialManager(PlayerInstance player) : BasePlayerManager(player)
{
    public HashSet<uint> ShowAvatarList { get; private set; } = [];
    public bool IsShowAvatar { get; set; }
    public Dictionary<uint, Friendship> Friends { get; private set; } = [];
    public Dictionary<uint, Friendship> PendingRequests { get; private set; } = [];
    public Dictionary<uint, Friendship> BlackList { get; private set; } = [];

    #region ShowAvatar
    public List<SocialShowAvatarInfo> GetSocialShowAvatarInfoList()
    {
        List<SocialShowAvatarInfo> showAvatarInfoList = [];

        if (ShowAvatarList.Count == 0 || Player.AvatarManager == null)
            return showAvatarInfoList;

        foreach (uint avatarId in ShowAvatarList)
        {
            var avatar = Player.AvatarManager.GetAvatar((int)avatarId);
            if (avatar == null) continue;

            showAvatarInfoList.Add(new SocialShowAvatarInfo
            {
                AvatarId = avatarId,
                Level = avatar.Level,
                CostumeId = 0
            });
        }

        return showAvatarInfoList;
    }
    #endregion

    #region SocialDetail
    public SocialDetail GetSocialDetail()
    {
        var socialShowAvatarInfoList = GetSocialShowAvatarInfoList();

        var social = new SocialDetail
        {
            Uid = (uint)Player.Uid,
            ProfilePicture = Player.Profile.HeadImage,
            Nickname = Player.Profile.Nickname ?? "",
            Signature = Player.Profile.Signature ?? "",
            Level = Player.Profile.Level,
            Birthday = Player.Profile.Birthday,
            WorldLevel = Player.Profile.WorldLevel,
            NameCardId = Player.Profile.NameCardId,
            IsShowAvatar = IsShowAvatar,
            FinishAchievementNum = Player.Profile.Achievements,
            OnlineState = FriendOnlineState.FriendOnline,
            FriendEnterHomeOption = FriendEnterHomeOption.NeedConfirm,
            IsMpModeAvailable = true,
            PlatformType = PlatformType.Pc
        };
        social.ShowAvatarInfoList.AddRange(socialShowAvatarInfoList);

        return social;
    }
    #endregion

    #region Friends
    public List<FriendBrief> GetFriendBriefList()
    {
        List<FriendBrief> friendList = [];
        foreach (var friendship in Friends.Values)
        {
            friendList.Add(friendship.ToProto());
        }
        return friendList;
    }

    public List<FriendBrief> GetPendingFriendBriefList()
    {
        List<FriendBrief> pendingList = [];
        foreach (var friendship in PendingRequests.Values)
        {
            pendingList.Add(friendship.ToProto());
        }
        return pendingList;
    }
    #endregion
}
