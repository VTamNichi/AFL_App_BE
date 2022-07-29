using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using Quartz;

namespace AmateurFootballLeague.ExternalService
{
    [DisallowConcurrentExecution]
    public class UnbanUserService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public UnbanUserService(IServiceProvider serviceProvider)
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

                IUserService userService = scope.ServiceProvider.GetService<IUserService>()!;

                List<User> listUser = userService.GetList().Where(u => u.DateUnban!.Value.CompareTo(currentDate) <= 0 && u.Status == false).ToList();
                foreach (User user in listUser)
                {
                    user.Status = true;
                    userService.UpdateAsync(user).Wait();
                }
            }

            return Task.CompletedTask;
        }
    }
}
