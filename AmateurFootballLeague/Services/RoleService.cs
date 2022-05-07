using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public IQueryable<Role> GetList(params Expression<Func<Role, object>>[] includes)
        {
            return _roleRepository.GetList(includes);
        }
        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }
        public async Task<Role> AddAsync(Role entity)
        {
            return await _roleRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Role entity)
        {
            return await _roleRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Role entity)
        {
            return await _roleRepository.DeleteAsync(entity);
        }

        // implement service method not CRUD here
    }
}
