using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Security.Cryptography;

namespace AmateurFootballLeague.Repositories
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
        public User GetUserByEmail(string email)
        {
            return GetList(x => x.Role).Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }
        public void EncryptPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}