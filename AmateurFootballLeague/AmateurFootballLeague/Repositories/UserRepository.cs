using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;

namespace AmateurFootballLeague.Repositories
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(DataContext dbContext) : base(dbContext) { }

        public User GetUserByEmail(string email)
        {
            return GetList(x => x.Role).Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }
    }
}