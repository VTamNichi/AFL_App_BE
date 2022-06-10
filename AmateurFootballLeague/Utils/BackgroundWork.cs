namespace AmateurFootballLeague.Utils
{
    public class BackgroundWork : BackgroundService
    {
        private readonly IWorker worker;

        public BackgroundWork(IWorker worker)
        {
            this.worker = worker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await worker.DoWork(stoppingToken);
        }
    }
}
