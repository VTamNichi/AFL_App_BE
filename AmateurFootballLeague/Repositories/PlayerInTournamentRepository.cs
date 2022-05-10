using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class PlayerInTournamentRepository:Repository<PlayerInTournament,int>, IplayerInTournament
    {
        public PlayerInTournamentRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
