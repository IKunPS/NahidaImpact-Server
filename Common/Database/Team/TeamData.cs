using NahidaImpact.Proto;
using NahidaImpact.Util;
using SqlSugar;

namespace NahidaImpact.Database.Team;

[SugarTable("Team")]
public class TeamData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true)] public List<TeamInfo> Teams { get; set; } = [];

    #region Instance Methods

    public TeamInfo? GetTeamById(int teamId)
        => Teams.Find(t => t.Index == teamId);

    public TeamInfo GetOrCreateTeam(int teamId)
    {
        var team = GetTeamById(teamId);
        if (team != null) return team;
        team = new TeamInfo { Index = teamId };
        Teams.Add(team);
        return team;
    }

    public void AddTeam(TeamInfo team)
    {
        if (Teams.Any(t => t.Index == team.Index))
            return;
        Teams.Add(team);
    }

    public void UpdateTeam(TeamInfo team)
    {
        var idx = Teams.FindIndex(t => t.Index == team.Index);
        if (idx >= 0)
            Teams[idx] = team;
        else
            Teams.Add(team);
    }

    public void RemoveTeam(int teamId)
        => Teams.RemoveAll(t => t.Index == teamId);

    #endregion

    #region Static Access

    public static TeamData? GetTeamDataByUid(int uid, bool forceReload = false)
    {
        return DatabaseHelper.GetInstance<TeamData>(uid, forceReload);
    }

    public static TeamData GetOrCreateTeamData(int uid)
    {
        return DatabaseHelper.GetInstanceOrCreateNew<TeamData>(uid);
    }

    public static void SaveTeamData(TeamData data)
    {
        DatabaseHelper.UpdateInstance(data);
    }

    public static TeamInfo? GetTeamByUidAndTeamId(int uid, int teamId)
    {
        var data = GetTeamDataByUid(uid);
        return data?.GetTeamById(teamId);
    }

    #endregion
}

public class TeamInfo
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public List<ulong> AvatarGuidList { get; set; } = [];

    public void CopyFrom(TeamInfo team, int maxTeamSize)
    {
        var guids = new List<ulong>(team.AvatarGuidList);
        AvatarGuidList.Clear();
        int len = Math.Min(guids.Count, maxTeamSize);
        for (int i = 0; i < len; i++)
            AvatarGuidList.Add(guids[i]);
    }

    public void CopyFrom(TeamInfo team)
        => CopyFrom(team, GameConstants.MAX_AVATARS_IN_TEAM);

    public AvatarTeam ToProto()
    {
        var proto = new AvatarTeam
        {
            TeamName = Name
        };
        foreach (var guid in AvatarGuidList)
            proto.AvatarGuidList.Add(guid);
        return proto;
    }
}