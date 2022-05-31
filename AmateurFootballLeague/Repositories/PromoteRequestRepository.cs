using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class PromoteRequestRepository : Repository<PromoteRequest, int>, IPromoteRequestRepository
    {
        public PromoteRequestRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
