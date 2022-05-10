using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class TeamInMatchRepository : Repository<TeamInMatch, int>, ITeamInMatchRepository
    {
        public TeamInMatchRepository(AmateurFootballLeagueContext dbContext) : base(dbContext)
        {
        }
    }
}
