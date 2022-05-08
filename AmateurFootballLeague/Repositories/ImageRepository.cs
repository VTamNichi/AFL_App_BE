using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class ImageRepository : Repository<Image, int>, IImageRepository
    {
        public ImageRepository(AmateurFootballLeagueContext dbContext) : base(dbContext) { }
    }
}
