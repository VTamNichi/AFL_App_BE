using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class TeamInTournamentService : ITeamInTournamentService
    {
        private readonly ITeamInTournamentRepository _teamInTournamentRepository;
        public TeamInTournamentService(ITeamInTournamentRepository teamInTournamentRepository)
        {
            _teamInTournamentRepository = teamInTournamentRepository;
        }
        public IQueryable<TeamInTournament> GetList(params Expression<Func<TeamInTournament, object>>[] includes)
        {
            return _teamInTournamentRepository.GetList(includes);
        }
        public async Task<TeamInTournament> GetByIdAsync(int id)
        {
            return await _teamInTournamentRepository.GetByIdAsync(id);
        }
        public async Task<TeamInTournament> AddAsync(TeamInTournament entity)
        {
            return await _teamInTournamentRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(TeamInTournament entity)
        {
            return await _teamInTournamentRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(TeamInTournament entity)
        {
            return await _teamInTournamentRepository.DeleteAsync(entity);
        }
        public int CountTeamInATournament(int tournamentId)
        {
            return _teamInTournamentRepository.CountTeamInATournament(tournamentId);
        }
    }
}
