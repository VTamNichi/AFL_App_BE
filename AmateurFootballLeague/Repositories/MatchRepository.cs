using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class MatchRepository : Repository<Match, int>, IMatchRepository
    {
        public MatchRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
