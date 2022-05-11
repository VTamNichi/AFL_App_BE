using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }
        public IQueryable<Tournament> GetList(params Expression<Func<Tournament, object>>[] includes)
        {
            return _tournamentRepository.GetList(includes);
        }
        public async Task<Tournament> GetByIdAsync(int id)
        {
            return await _tournamentRepository.GetByIdAsync(id);
        }
        public async Task<Tournament> AddAsync(Tournament entity)
        {
            return await _tournamentRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Tournament entity)
        {
            return await _tournamentRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Tournament entity)
        {
            return await _tournamentRepository.DeleteAsync(entity);
        }
        public int CountAllTournament()
        {
            return _tournamentRepository.CountAllTournament();
        }
    }
}
