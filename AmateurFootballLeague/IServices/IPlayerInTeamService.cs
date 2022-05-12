using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IServices
{
    public interface IPlayerInTeamService: IService<PlayerInTeam, int>
    {
        int CountPlayerInATeam(int teamId);
    }
}
