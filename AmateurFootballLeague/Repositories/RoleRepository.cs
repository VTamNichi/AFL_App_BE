using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Repositories
{
    public class RoleRepository : Repository<Role, int>, IRoleRepository
    {
        public RoleRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }

        // implement repository method not CRUD here
    }
}
