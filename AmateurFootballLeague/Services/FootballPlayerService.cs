using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class FootballPlayerService : IFootballPlayerService
    {
        private readonly IFootballPlayerRepository _footballPlayerRepository;
        public FootballPlayerService(IFootballPlayerRepository footballPlayerRepository)
        {
            _footballPlayerRepository = footballPlayerRepository;
        }
        public IQueryable<FootballPlayer> GetList(params Expression<Func<FootballPlayer, object>>[] includes)
        {
            return _footballPlayerRepository.GetList(includes);
        }
        public async Task<FootballPlayer> GetByIdAsync(int id)
        {
            return await _footballPlayerRepository.GetByIdAsync(id);
        }
        public async Task<FootballPlayer> AddAsync(FootballPlayer entity)
        {
            return await _footballPlayerRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(FootballPlayer entity)
        {
            return await _footballPlayerRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(FootballPlayer entity)
        {
            return await _footballPlayerRepository.DeleteAsync(entity);
        }
    }
}
