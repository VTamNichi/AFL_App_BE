using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TournamentResultService : ITournamentResultService
    {
        private readonly ITournamentResultRepository _tournamentResult;

        public TournamentResultService(ITournamentResultRepository tournamentResult)
        {
            _tournamentResult = tournamentResult;
        }
        public async Task<TournamentResult> AddAsync(TournamentResult entity)
        {
            return await _tournamentResult.AddAsync(entity);
        }
        public async Task<bool> DeleteAsync(TournamentResult entity)
        {
            return await _tournamentResult.DeleteAsync(entity);
        }

        public async Task<TournamentResult> GetByIdAsync(int id)
        {
            return await _tournamentResult.GetByIdAsync(id);
        }

        public IQueryable<TournamentResult> GetList(params Expression<Func<TournamentResult, object>>[] includes)
        {
            return _tournamentResult.GetList(includes);
        }

        public async Task<bool> UpdateAsync(TournamentResult entity)
        {
            return await _tournamentResult.UpdateAsync(entity);
        }
    }
}
