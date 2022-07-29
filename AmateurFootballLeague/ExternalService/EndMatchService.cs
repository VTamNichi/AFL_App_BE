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
                DateTime currentDate = DateTime.Now.AddHours(7);

                IMatchService matchService = scope.ServiceProvider.GetService<IMatchService>()!;

                List<Match> listStart = matchService.GetList().Where(m => m.MatchDate!.Value.CompareTo(currentDate) <= 0 && m.Status == "Chưa bắt đầu").ToList();
                foreach (Match match in listStart)
                {
                    match.Status = "Đang diễn ra";
                    matchService.UpdateAsync(match).Wait();
                }

                List<Match> listEnd = matchService.GetList().Where(m => m.MatchDate!.Value.AddHours(1).CompareTo(currentDate) <= 0 && m.Status == "Đang diễn ra").ToList();
                foreach (Match match in listEnd)
                {
                    match.Status = "Kết thúc";
                    matchService.UpdateAsync(match).Wait();
                }
            }

            return Task.CompletedTask;
        }
    }
}
