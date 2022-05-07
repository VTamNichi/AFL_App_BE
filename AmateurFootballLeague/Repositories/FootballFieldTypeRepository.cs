using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class FootballFieldTypeRepository : Repository<FootballFieldType, int>, IFootballFieldTypeRepository
    {
        public FootballFieldTypeRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
