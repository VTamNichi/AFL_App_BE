using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class ActionMatchRepository : Repository<ActionMatch, int>, IActionMatchRepository
    {
        public ActionMatchRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
