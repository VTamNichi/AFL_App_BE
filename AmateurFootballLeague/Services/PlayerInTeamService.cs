using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class PlayerInTeamService : IPlayerInTeamService
    {
        public readonly IPlayerInTeamRepository _playerInTeam;
        public PlayerInTeamService(IPlayerInTeamRepository playerInTeam)
        {
            _playerInTeam = playerInTeam;   
        }
        public async Task<PlayerInTeam> AddAsync(PlayerInTeam entity)
        {
             return await _playerInTeam.AddAsync(entity);  
        }

        public Task<bool> DeleteAsync(PlayerInTeam entity)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerInTeam> GetByIdAsync(int id)
        {
           return  await _playerInTeam.GetByIdAsync(id);   
        }

        public IQueryable<PlayerInTeam> GetList(params Expression<Func<PlayerInTeam, object>>[] includes)
        {
           return _playerInTeam.GetList(includes);
        }

        public async Task<bool> UpdateAsync(PlayerInTeam entity)
        {
            return await _playerInTeam.UpdateAsync(entity);
        }
    }
}
