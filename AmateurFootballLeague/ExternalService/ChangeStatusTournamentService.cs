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
                TimeSpan totalTime = DateTime.Now.TimeOfDay;
                if(totalTime.Hours >= 17)
                {
                    currentDate = currentDate.AddDays(1);
                }

                ITournamentService tournamentService = scope.ServiceProvider.GetService<ITournamentService>()!;
                List<Tournament> listTournamentStart = tournamentService.GetList().Where(t => t.StatusTnm == "Chuẩn bị" && t.TournamentStartDate!.Value.CompareTo(currentDate) >= 0).ToList();
                if (listTournamentStart != null && listTournamentStart.Count > 0)
                {
                    foreach(Tournament tournament in listTournamentStart)
                    {
                        tournament.StatusTnm = "Đang diễn ra";
                        tournamentService.UpdateAsync(tournament).Wait();
                    }
                }

                List<Tournament> listTournamentEnd = tournamentService.GetList().Where(t => t.StatusTnm == "Đang diễn ra" && t.TournamentEndDate!.Value.CompareTo(currentDate) < 0).ToList();
                if (listTournamentEnd != null && listTournamentEnd.Count > 0)
                {
                    foreach (Tournament tournament in listTournamentEnd)
                    {
                        tournament.StatusTnm = "Kết thúc";
                        tournamentService.UpdateAsync(tournament).Wait();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
