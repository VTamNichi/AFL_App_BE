using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class ScorePredictionService : IScorePredictionService
    {
        private readonly IScorePredictionRepository _scorePrediction;

        public ScorePredictionService(IScorePredictionRepository scorePrediction)
        {
            _scorePrediction = scorePrediction;
        }
        public async Task<ScorePrediction> AddAsync(ScorePrediction entity)
        {
            return await _scorePrediction.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(ScorePrediction entity)
        {
            return await _scorePrediction.DeleteAsync(entity);
        }

        public async Task<ScorePrediction> GetByIdAsync(int id)
        {
            return await _scorePrediction.GetByIdAsync(id);
        }

        public IQueryable<ScorePrediction> GetList(params Expression<Func<ScorePrediction, object>>[] includes)
        {
            return _scorePrediction.GetList(includes);
        }

        public async Task<bool> UpdateAsync(ScorePrediction entity)
        {
            return await _scorePrediction.UpdateAsync(entity);
        }
    }
}
