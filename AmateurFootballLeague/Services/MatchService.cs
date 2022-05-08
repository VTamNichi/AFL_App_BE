using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }
        public IQueryable<Match> GetList(params Expression<Func<Match, object>>[] includes)
        {
            return _matchRepository.GetList(includes);
        }
        public async Task<Match> GetByIdAsync(int id)
        {
            return await _matchRepository.GetByIdAsync(id);
        }
        public async Task<Match> AddAsync(Match entity)
        {
            return await _matchRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Match entity)
        {
            return await _matchRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Match entity)
        {
            return await _matchRepository.DeleteAsync(entity);
        }
    }
}
