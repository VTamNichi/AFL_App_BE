using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TournamentTypeService : ITournamentTypeService
    {
        private readonly ITournamentTypeRepository _tournamentTypeRepository;
        public TournamentTypeService(ITournamentTypeRepository tournamentTypeRepository)
        {
            _tournamentTypeRepository = tournamentTypeRepository;
        }
        public IQueryable<TournamentType> GetList(params Expression<Func<TournamentType, object>>[] includes)
        {
            return _tournamentTypeRepository.GetList(includes);
        }
        public async Task<TournamentType> GetByIdAsync(int id)
        {
            return await _tournamentTypeRepository.GetByIdAsync(id);
        }
        public async Task<TournamentType> AddAsync(TournamentType entity)
        {
            return await _tournamentTypeRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(TournamentType entity)
        {
            return await _tournamentTypeRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(TournamentType entity)
        {
            return await _tournamentTypeRepository.DeleteAsync(entity);
        }
    }
}
