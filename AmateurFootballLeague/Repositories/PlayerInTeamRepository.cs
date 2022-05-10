

using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class PlayerInTeamRepository: Repository<PlayerInTeam, int>, IPlayerInTeamRepository
    {
        public PlayerInTeamRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
