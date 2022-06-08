using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class ReportRepository : Repository<Report, int>, IReportRepository
    {
        public ReportRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
