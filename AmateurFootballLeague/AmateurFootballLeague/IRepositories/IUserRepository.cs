using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.IRepositories
{
    public interface IUserRepository : IRepository<User, int>
    {
        User GetUserByEmail(string email);
    }
}