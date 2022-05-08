using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class FootballPlayerRepository : Repository<FootballPlayer, int>, IFootballPlayerRepository
    {
        public FootballPlayerRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
