using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;

namespace AmateurFootballLeague.Repositories
{
    public class RoleRepository : Repository<Role, int>, IRoleRepository
    {
        public RoleRepository(DataContext dbContext) : base(dbContext) {}

        // implement repository method not CRUD here
    }
}
