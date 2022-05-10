using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class PlayerInTournamentService : IPlayerInTournamentService
    {
        private readonly IplayerInTournament _playerInTournament;

        public PlayerInTournamentService(IplayerInTournament playerInTournament)
        {
            _playerInTournament = playerInTournament;
        }
        public async Task<PlayerInTournament> AddAsync(PlayerInTournament entity)
        {
            return await _playerInTournament.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(PlayerInTournament entity)
        {
            return await _playerInTournament.DeleteAsync(entity);
        }

        public async Task<PlayerInTournament> GetByIdAsync(int id)
        {
            return await _playerInTournament.GetByIdAsync(id);
        }

        public IQueryable<PlayerInTournament> GetList(params Expression<Func<PlayerInTournament, object>>[] includes)
        {
            return _playerInTournament.GetList(includes);
        }

        public async Task<bool> UpdateAsync(PlayerInTournament entity)
        {
            return await _playerInTournament.UpdateAsync(entity);  
        }
    }
}
