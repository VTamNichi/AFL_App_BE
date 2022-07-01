using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class ActionMatchService : IActionMatchService
    {
        private readonly IActionMatchRepository _actionMatchRepository;
        public ActionMatchService(IActionMatchRepository actionMatchRepository)
        {
            _actionMatchRepository = actionMatchRepository;
        }
        public IQueryable<ActionMatch> GetList(params Expression<Func<ActionMatch, object>>[] includes)
        {
            return _actionMatchRepository.GetList(includes);
        }
        public async Task<ActionMatch> GetByIdAsync(int id)
        {
            return await _actionMatchRepository.GetByIdAsync(id);
        }
        public async Task<ActionMatch> AddAsync(ActionMatch entity)
        {
            return await _actionMatchRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(ActionMatch entity)
        {
            return await _actionMatchRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(ActionMatch entity)
        {
            return await _actionMatchRepository.DeleteAsync(entity);
        }
    }
}
