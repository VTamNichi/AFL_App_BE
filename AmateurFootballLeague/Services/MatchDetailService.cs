using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class MatchDetailService : IMatchDetailService
    {
        private readonly IMatchDetailRepository _matchDetailRepository;

        public MatchDetailService(IMatchDetailRepository matchDetailRepository)
        {
            _matchDetailRepository = matchDetailRepository;
        }
        public async Task<MatchDetail> AddAsync(MatchDetail entity)
        {
            return await _matchDetailRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(MatchDetail entity)
        {
            return await _matchDetailRepository.DeleteAsync(entity);
        }

        public async Task<MatchDetail> GetByIdAsync(int id)
        {
            return await _matchDetailRepository.GetByIdAsync(id);
        }

        public IQueryable<MatchDetail> GetList(params Expression<Func<MatchDetail, object>>[] includes)
        {
            return _matchDetailRepository.GetList(includes);
        }

        public async Task<bool> UpdateAsync(MatchDetail entity)
        {
            return await _matchDetailRepository.UpdateAsync(entity);
        }
    }
}
