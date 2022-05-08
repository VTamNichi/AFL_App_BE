using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class TeamRepository : Repository<Team, int>, ITeamRepository
    {
        public TeamRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
