using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IRepositories
{
    public interface ITeamInTournamentRepository : IRepository<TeamInTournament, int>
    {
        int CountTeamInATournament(int tournamentId);
    }
}
