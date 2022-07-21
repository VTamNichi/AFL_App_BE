using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using Quartz;

namespace AmateurFootballLeague.ExternalService
{
    [DisallowConcurrentExecution]
    public class EndMatchService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public EndMatchService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ITournamentService tournamentService = scope.ServiceProvider.GetService<ITournamentService>()!;
                IMatchService matchService = scope.ServiceProvider.GetService<IMatchService>()!;
                ITeamInMatchService teamInMatchService = scope.ServiceProvider.GetService<ITeamInMatchService>()!;


                List<Tournament> listTournament = tournamentService.GetList().Where(t => t.TournamentTypeId == 1).ToList();
                foreach (Tournament tournament in listTournament)
                {
                    List<Match> listMatch = matchService.GetList().Where(m => m.TournamentId == tournament.Id).ToList();
                    foreach (Match match in listMatch)
                    {
                        TeamInMatch teamInMatchWin = new();
                        List<TeamInMatch> listTeamInMatch = teamInMatchService.GetList().Where(tim => tim.MatchId == match.Id).ToList();
                        if (listTeamInMatch[0].Result > 1)
                        {
                            teamInMatchWin = listTeamInMatch[0];
                        }
                        if (listTeamInMatch[1].Result > 1)
                        {
                            teamInMatchWin = listTeamInMatch[1];
                        }
                       
                        if (teamInMatchWin != null)
                        {
                            Match matchWin = matchService.GetList().Where(m => m.Id == teamInMatchWin.MatchId!.Value).FirstOrDefault()!;
                            TeamInMatch teamInMatchNext = teamInMatchService.GetList().Where(tim => tim.Match.TournamentId == tournament.Id && tim.TeamName == "Thắng " + matchWin.Fight).FirstOrDefault()!;
                            if(teamInMatchNext != null)
                            {
                                teamInMatchNext.NextTeam = "";
                                teamInMatchNext.TeamName = teamInMatchWin.TeamName;
                                teamInMatchNext.TeamInTournamentId = teamInMatchWin.TeamInTournamentId;
                                teamInMatchService.UpdateAsync(teamInMatchNext).Wait();
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
