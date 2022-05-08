using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class TeamInTournamentRepository : Repository<TeamInTournament, int>, ITeamInTournamentRepository
    {
        public TeamInTournamentRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
