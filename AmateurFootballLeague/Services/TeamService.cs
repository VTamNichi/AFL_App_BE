using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public IQueryable<Team> GetList(params Expression<Func<Team, object>>[] includes)
        {
            return _teamRepository.GetList(includes);
        }
        public async Task<Team> GetByIdAsync(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }
        public async Task<Team> AddAsync(Team entity)
        {
            return await _teamRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Team entity)
        {
            return await _teamRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Team entity)
        {
            return await _teamRepository.DeleteAsync(entity);
        }
        public int CountAllTeam()
        {
            return _teamRepository.CountAllTeam(); 
        }
    }
}
