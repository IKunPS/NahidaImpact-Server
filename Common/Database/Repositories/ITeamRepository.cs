using NahidaImpact.Database.Team;

namespace NahidaImpact.Database.Repositories;

public interface ITeamRepository
{
    Task<TeamData> GetOrCreateAsync(int uid);
    Task UpdateAsync(TeamData teamData);
    Task DeleteAsync(int uid);
}