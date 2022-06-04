

using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class PlayerInTeamRepository: Repository<PlayerInTeam, int>, IPlayerInTeamRepository
    {
        public PlayerInTeamRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }

        public int CountPlayerInATeam(int teamId)
        {
            return GetList().Where(pit => pit.TeamId == teamId && pit.Status == "true").Count();
        }
    }
}
