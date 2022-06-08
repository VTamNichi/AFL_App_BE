using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class NotificationRepository : Repository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
