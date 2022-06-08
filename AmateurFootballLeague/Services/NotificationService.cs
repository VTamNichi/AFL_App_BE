using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public IQueryable<Notification> GetList(params Expression<Func<Notification, object>>[] includes)
        {
            return _notificationRepository.GetList(includes);
        }
        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }
        public async Task<Notification> AddAsync(Notification entity)
        {
            return await _notificationRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Notification entity)
        {
            return await _notificationRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Notification entity)
        {
            return await _notificationRepository.DeleteAsync(entity);
        }
    }
}
