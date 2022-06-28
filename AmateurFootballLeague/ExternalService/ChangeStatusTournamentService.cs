using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using Quartz;

namespace AmateurFootballLeague.ExternalService
{
    [DisallowConcurrentExecution]
    public class ChangeStatusTournamentService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public ChangeStatusTournamentService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                DateTime currentDate = DateTime.Today;
                
                ITournamentService tournamentService = scope.ServiceProvider.GetService<ITournamentService>()!;
                List<Tournament> listTournament = tournamentService.GetList().Where(t => t.StatusTnm == "Chuẩn bị" && t.TournamentStartDate!.Value.CompareTo(currentDate) >= 0).ToList();

                if (listTournament != null && listTournament.Count > 0)
                {
                    foreach(Tournament tournament in listTournament)
                    {
                        tournament.StatusTnm = "Đang diễn ra";
                        tournamentService.UpdateAsync(tournament).Wait();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
