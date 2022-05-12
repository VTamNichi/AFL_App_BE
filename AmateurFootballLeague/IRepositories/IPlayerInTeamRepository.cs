using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IRepositories
{
    public interface IPlayerInTeamRepository : IRepository<PlayerInTeam, int>
    {
        int CountPlayerInATeam(int teamId);
    }
}
