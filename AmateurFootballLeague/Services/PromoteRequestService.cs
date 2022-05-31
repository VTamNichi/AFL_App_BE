using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class PromoteRequestService : IPromoteRequestService
    {
        private readonly IPromoteRequestRepository _promoteRequestRepository;
        public PromoteRequestService(IPromoteRequestRepository promoteRequestRepository)
        {
            _promoteRequestRepository = promoteRequestRepository;
        }
        public IQueryable<PromoteRequest> GetList(params Expression<Func<PromoteRequest, object>>[] includes)
        {
            return _promoteRequestRepository.GetList(includes);
        }
        public async Task<PromoteRequest> GetByIdAsync(int id)
        {
            return await _promoteRequestRepository.GetByIdAsync(id);
        }
        public async Task<PromoteRequest> AddAsync(PromoteRequest entity)
        {
            return await _promoteRequestRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(PromoteRequest entity)
        {
            return await _promoteRequestRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(PromoteRequest entity)
        {
            return await _promoteRequestRepository.DeleteAsync(entity);
        }
    }
}
