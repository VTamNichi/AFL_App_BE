using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class ScorePredictionRepository : Repository<ScorePrediction, int>, IScorePredictionRepository
    {
        public ScorePredictionRepository(AmateurFootballLeagueContext dbContext) : base(dbContext)
        {
        }
    }
}
