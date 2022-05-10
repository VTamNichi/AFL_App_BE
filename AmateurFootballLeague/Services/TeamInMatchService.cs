using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TeamInMatchService : ITeamInMatchService
    {
        private readonly ITeamInMatchRepository _teamInMatchRepository;

        public TeamInMatchService(ITeamInMatchRepository teamInMatchRepository)
        {
            _teamInMatchRepository = teamInMatchRepository; 
        }
        public async Task<TeamInMatch> AddAsync(TeamInMatch entity)
        {
            return await _teamInMatchRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(TeamInMatch entity)
        {
            return await _teamInMatchRepository.DeleteAsync(entity);
        }

        public async Task<TeamInMatch> GetByIdAsync(int id)
        {
            return await _teamInMatchRepository.GetByIdAsync(id);
        }

        public IQueryable<TeamInMatch> GetList(params Expression<Func<TeamInMatch, object>>[] includes)
        {
           return _teamInMatchRepository.GetList(includes);
        }

        public async Task<bool> UpdateAsync(TeamInMatch entity)
        {
            return await _teamInMatchRepository.UpdateAsync(entity);
        }
    }
}
