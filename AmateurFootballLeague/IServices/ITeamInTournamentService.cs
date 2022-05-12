using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IServices
{
    public interface ITeamInTournamentService : IService<TeamInTournament, int>
    {
        int CountTeamInATournament(int tournamentId);
    }
}
