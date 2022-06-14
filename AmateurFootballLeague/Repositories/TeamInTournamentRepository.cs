using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class TeamInTournamentRepository : Repository<TeamInTournament, int>, ITeamInTournamentRepository
    {
        public TeamInTournamentRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }

        public int CountTeamInATournament(int tournamentId)
        {
            return GetList().Where(tit => tit.TournamentId == tournamentId && tit.Status == "Tham gia").Count();
        }
    }
}
