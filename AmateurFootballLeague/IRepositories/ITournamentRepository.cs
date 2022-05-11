using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IRepositories
{
    public interface ITournamentRepository : IRepository<Tournament, int>
    {
        int CountAllTournament();
    }
}
