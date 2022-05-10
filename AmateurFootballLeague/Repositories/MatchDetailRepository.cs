using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class MatchDetailRepository : Repository<MatchDetail, int>, IMatchDetailRepository
    {
        public MatchDetailRepository(AmateurFootballLeagueContext dbContext) : base(dbContext)
        {
        }
    }
}
