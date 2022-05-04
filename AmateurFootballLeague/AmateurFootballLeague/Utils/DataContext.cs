using AmateurFootballLeague.Models;
using Microsoft.EntityFrameworkCore;

namespace AmateurFootballLeague.Utils
{
    #pragma warning disable CS1591
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
#pragma warning disable CS1591
}
