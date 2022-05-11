using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class TournamentRepository : Repository<Tournament, int>, ITournamentRepository
    {
        public TournamentRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }

        public int CountAllTournament()
        {
            return CountAsync();
        }
    }
}
