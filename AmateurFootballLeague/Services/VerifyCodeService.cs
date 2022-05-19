using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class VerifyCodeService : IVerifyCodeService
    {
        private readonly IVerifyCodeRepository _verifyCodeRepository;
        public VerifyCodeService(IVerifyCodeRepository verifyCodeRepository)
        {
            _verifyCodeRepository = verifyCodeRepository;
        }
        public IQueryable<VerifyCode> GetList(params Expression<Func<VerifyCode, object>>[] includes)
        {
            return _verifyCodeRepository.GetList(includes);
        }
        public async Task<VerifyCode> GetByIdAsync(int id)
        {
            return await _verifyCodeRepository.GetByIdAsync(id);
        }
        public async Task<VerifyCode> AddAsync(VerifyCode entity)
        {
            return await _verifyCodeRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(VerifyCode entity)
        {
            return await _verifyCodeRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(VerifyCode entity)
        {
            return await _verifyCodeRepository.DeleteAsync(entity);
        }
    }
}