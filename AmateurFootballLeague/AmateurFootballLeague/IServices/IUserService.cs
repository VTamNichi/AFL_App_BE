using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IServices
{
    public interface IUserService : IService<User, int>
    {
        User GetUserByEmail(string email);
    }
}