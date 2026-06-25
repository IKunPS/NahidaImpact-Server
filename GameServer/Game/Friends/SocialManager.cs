using NahidaImpact.GameServer.Game.Avatar;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.GameServer.Server.Packet.Send.Player;
using NahidaImpact.Proto;
using NahidaImpact.Util.Extensions;

namespace NahidaImpact.GameServer.Game.Friends;

public class SocialManager(PlayerInstance player) : BasePlayerManager(player)
{
    public HashSet<uint> ShowAvatarList { get; private set; } = [];
    public HashSet<uint> ShowNameCardList { get; private set; } = [];
    public bool IsShowAvatar { get; set; }
    public Dictionary<uint, Friendship> Friends { get; private set; } = [];
    public Dictionary<uint, Friendship> PendingRequests { get; private set; } = [];
    public Dictionary<uint, Friendship> BlackList { get; private set; } = [];

    // Load persisted showcase data
    public void LoadFromData()
    {
        if (Player.Data.ShowAvatarIdList != null)
            ShowAvatarList = [..Player.Data.ShowAvatarIdList];
        if (Player.Data.ShowNameCardIdList != null)
            ShowNameCardList = [..Player.Data.ShowNameCardIdList];
    }

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
                CostumeId = avatar.CostumeId
            });
        }

        return showAvatarInfoList;
    }
    #endregion

    #region SocialDetail

    // Build SocialDetail for the local player (matches hk4e PlayerSocialComp::fillSocialDetail)
    public SocialDetail GetSocialDetail(uint targetUid = 0)
    {
        var showAvatarInfoList = GetSocialShowAvatarInfoList();

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
        social.ShowAvatarInfoList.AddRange(showAvatarInfoList);
        social.ShowNameCardIdList.AddRange(ShowNameCardList);

        // Friend/blacklist checks for cross-player lookup
        if (targetUid > 0 && targetUid != Player.Uid)
        {
            social.IsFriend = Friends.ContainsKey(targetUid);
            social.IsInBlacklist = BlackList.ContainsKey(targetUid);
        }

        return social;
    }
    #endregion

    #region Friends

    public List<FriendBrief> GetFriendBriefList()
    {
        List<FriendBrief> friendList = [];
        foreach (var friendship in Friends.Values)
            friendList.Add(friendship.ToProto());
        return friendList;
    }

    public List<FriendBrief> GetPendingFriendBriefList()
    {
        List<FriendBrief> pendingList = [];
        foreach (var friendship in PendingRequests.Values)
            pendingList.Add(friendship.ToProto());
        return pendingList;
    }
    #endregion

    #region Profile Mutations

    // Sets the player signature and persists to DB
    public void SetSignature(string signature)
    {
        Player.Profile.Signature = signature ?? "";
        Player.Data.Signature = Player.Profile.Signature;
        Player.Data.Save();
    }

    // Sets the active name card; validates it is unlocked
    public bool SetNameCard(uint nameCardId)
    {
        if (nameCardId > 0 && !Player.Data.NameCardList.Contains((int)nameCardId))
            return false;
        Player.Profile.NameCardId = nameCardId;
        Player.Data.NameCardId = (int)nameCardId;
        Player.Data.Save();
        return true;
    }

    // Unlocks a namecard and notifies the client. Auto-equips if none is set yet (hk4e PlayerSocialComp::unlockNameCard)
    public bool UnlockNameCard(uint nameCardId)
    {
        var list = Player.Data.NameCardList;
        if (list == null!)
        {
            Player.Data.NameCardList = [];
            list = Player.Data.NameCardList;
        }

        if (list.Contains((int)nameCardId))
            return false;

        list.Add((int)nameCardId);
        Player.Data.Save();

        // Auto-equip the first unlocked namecard
        if (Player.Profile.NameCardId == 0)
        {
            Player.Profile.NameCardId = nameCardId;
            Player.Data.NameCardId = (int)nameCardId;
        }

        _ = Player.SendPacket(new PacketUnlockNameCardNotify(nameCardId));
        return true;
    }

    // Sets the profile picture (head image) from an owned avatar
    public void SetHeadImage(uint avatarId, uint costumeId = 0)
    {
        Player.Profile.HeadImage = new ProfilePicture
        {
            ProfilePictureId = avatarId,
            CostumeId = costumeId
        };
        Player.Data.ProfilePictureAvatarId = avatarId;
        Player.Data.ProfilePictureCostumeId = costumeId;
        Player.Data.Save();
    }

    // Updates the avatar showcase list
    public void UpdateShowAvatarList(IEnumerable<uint> avatarIds, bool isShowAvatar)
    {
        ShowAvatarList = [..avatarIds];
        IsShowAvatar = isShowAvatar;
        Player.Data.ShowAvatarIdList = [..ShowAvatarList];
        Player.Data.Save();
    }

    // Updates the name card showcase list
    public void UpdateShowNameCardList(IEnumerable<uint> nameCardIds)
    {
        ShowNameCardList = [..nameCardIds];
        Player.Data.ShowNameCardIdList = [..ShowNameCardList];
        Player.Data.Save();
    }

    // Sets the player birthday
    public void SetBirthday(uint month, uint day)
    {
        Player.Profile.Birthday = new Birthday { Month = month, Day = day };
        Player.Data.SetBirthday(month, day);
        Player.Data.Save();
    }

    #endregion
}
