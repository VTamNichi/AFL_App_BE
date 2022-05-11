using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class TournamentResultRepository : Repository<TournamentResult, int>, ITournamentResultRepository
    {
        public TournamentResultRepository(AmateurFootballLeagueContext dbContext) : base(dbContext)
        {
        }
    }
}
