using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IQueryable<User> GetList(params Expression<Func<User, object>>[] includes)
        {
            return _userRepository.GetList(includes);
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
        public async Task<User> AddAsync(User entity)
        {
            return await _userRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(User entity)
        {
            return await _userRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(User entity)
        {
            return await _userRepository.DeleteAsync(entity);
        }
        public User GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }
        public List<byte[]> EncriptPassword(string password)
        {
            _userRepository.EncryptPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            List<byte[]> listPassword = new List<byte[]>();
            listPassword.Add(passwordHash);
            listPassword.Add(passwordSalt);
            return listPassword;
        }
        public bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            return _userRepository.CheckPassword(password, passwordHash, passwordSalt);
        }
    }
}