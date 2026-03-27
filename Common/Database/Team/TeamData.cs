using SqlSugar;
using System.Collections.Generic;

namespace NahidaImpact.Database.Team;

[SugarTable("Team")]
public class TeamData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<TeamInfo> Teams { get; set; } = [];

    [SugarColumn(IsJson = true)] public TeamInfo MpTeam { get; set; } = new();

    [SugarColumn(IsJson = true)] public List<TeamInfo> TemporaryTeams { get; set; } = [];
    
    [SugarColumn(IsJson = true)] public TeamInfo? TrialAvatarTeam { get; set; } = new TeamInfo();

    [SugarColumn(IsJson = true)] public Dictionary<int, uint> TrialAvatars { get; set; } = [];

    public int PreviousIndex { get; set; } = -1;
    
    public int UseTemporarilyTeamIndex { get; set; } = -1;

    public bool UsingTrialTeam { get; set; }
    
    public uint CurrentTeamIndex { get; set; }

}

public class TeamInfo
{
    public uint Index { get; set; }
    public string Name { get; set; } = "";
    public List<ulong> AvatarGuidList { get; set; } = [];

    public TeamInfo() { }

    public TeamInfo(List<ulong> avatarGuidList)
    {
        AvatarGuidList = avatarGuidList;
    }

    public int Size => AvatarGuidList.Count;

    public bool Contains(ulong avatarGuid) => AvatarGuidList.Contains(avatarGuid);

    public bool AddAvatar(ulong avatarGuid)
    {
        if (Contains(avatarGuid))
            return false;

        AvatarGuidList.Add(avatarGuid);
        return true;
    }

    public bool RemoveAvatar(int slot)
    {
        if (Size <= 1)
            return false;

        AvatarGuidList.RemoveAt(slot);
        return true;
    }

    public void CopyFrom(TeamInfo team, int maxTeamSize)
    {
        var avatarGuids = new List<ulong>(team.AvatarGuidList);
        AvatarGuidList.Clear();

        int len = System.Math.Min(avatarGuids.Count, maxTeamSize);
        for (int i = 0; i < len; i++)
        {
            AvatarGuidList.Add(avatarGuids[i]);
        }
    }
}