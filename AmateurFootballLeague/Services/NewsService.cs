using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }
        public IQueryable<News> GetList(params Expression<Func<News, object>>[] includes)
        {
            return _newsRepository.GetList(includes);
        }
        public async Task<News> GetByIdAsync(int id)
        {
            return await _newsRepository.GetByIdAsync(id);
        }
        public async Task<News> AddAsync(News entity)
        {
            return await _newsRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(News entity)
        {
            return await _newsRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(News entity)
        {
            return await _newsRepository.DeleteAsync(entity);
        }
    }
}
