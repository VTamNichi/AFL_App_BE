using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class NewsRepository : Repository<News, int>, INewsRepository
    {
        public NewsRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
