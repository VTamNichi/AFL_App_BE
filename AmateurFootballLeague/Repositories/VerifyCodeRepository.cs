using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Security.Cryptography;

namespace AmateurFootballLeague.Repositories
{
    public class VerifyCodeRepository : Repository<VerifyCode, int>, IVerifyCodeRepository
    {
        public VerifyCodeRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}