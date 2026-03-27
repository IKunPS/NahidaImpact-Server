using NahidaImpact.Database.Team;
using NahidaImpact.Util;
using SqlSugar;

namespace NahidaImpact.Database.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ISqlSugarClient _db;

    public TeamRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<TeamData> GetOrCreateAsync(int uid)
    {
        var teamData = await _db.Queryable<TeamData>().FirstAsync(t => t.Uid == uid);
        if (teamData != null)
            return teamData;

        teamData = new TeamData { Uid = uid };
        // 初始化默认队伍
        for (uint i = 1; i <= GameConstants.DEFAULT_TEAMS; i++)
            teamData.Teams.Add(new TeamInfo { Index = i });
        teamData.CurrentTeamIndex = 1;
        teamData.MpTeam = new TeamInfo();
        teamData.TemporaryTeams = [];
        teamData.TrialAvatarTeam = new TeamInfo();
        teamData.TrialAvatars = [];
        await _db.Insertable(teamData).ExecuteCommandAsync();
        return teamData;
    }

    public async Task UpdateAsync(TeamData teamData)
    {
        await _db.Updateable(teamData).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int uid)
    {
        await _db.Deleteable<TeamData>().Where(t => t.Uid == uid).ExecuteCommandAsync();
    }
}