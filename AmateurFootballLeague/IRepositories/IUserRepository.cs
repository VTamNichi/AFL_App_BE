using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IRepositories
{
    public interface IUserRepository : IRepository<User, int>
    {
        User GetUserByEmail(string email);
        void EncryptPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);

        bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}