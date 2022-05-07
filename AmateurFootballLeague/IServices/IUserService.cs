using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IServices
{
    public interface IUserService : IService<User, int>
    {
        User GetUserByEmail(string email);
        List<byte[]> EncriptPassword(string password);
        bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}