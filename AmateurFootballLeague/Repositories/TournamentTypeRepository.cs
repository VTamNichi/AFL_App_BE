using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class TournamentTypeRepository : Repository<TournamentType, int>, ITournamentTypeRepository
    {
        public TournamentTypeRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
