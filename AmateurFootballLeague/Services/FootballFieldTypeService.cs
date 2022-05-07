using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class FootballFieldTypeService : IFootballFieldTypeService
    {
        private readonly IFootballFieldTypeRepository _footballFieldTypeRepository;
        public FootballFieldTypeService(IFootballFieldTypeRepository footballFieldTypeRepository)
        {
            _footballFieldTypeRepository = footballFieldTypeRepository;
        }
        public IQueryable<FootballFieldType> GetList(params Expression<Func<FootballFieldType, object>>[] includes)
        {
            return _footballFieldTypeRepository.GetList(includes);
        }
        public async Task<FootballFieldType> GetByIdAsync(int id)
        {
            return await _footballFieldTypeRepository.GetByIdAsync(id);
        }
        public async Task<FootballFieldType> AddAsync(FootballFieldType entity)
        {
            return await _footballFieldTypeRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(FootballFieldType entity)
        {
            return await _footballFieldTypeRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(FootballFieldType entity)
        {
            return await _footballFieldTypeRepository.DeleteAsync(entity);
        }
    }
}
