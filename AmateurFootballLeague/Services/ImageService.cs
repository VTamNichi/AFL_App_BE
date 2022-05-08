using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }
        public IQueryable<Image> GetList(params Expression<Func<Image, object>>[] includes)
        {
            return _imageRepository.GetList(includes);
        }
        public async Task<Image> GetByIdAsync(int id)
        {
            return await _imageRepository.GetByIdAsync(id);
        }
        public async Task<Image> AddAsync(Image entity)
        {
            return await _imageRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Image entity)
        {
            return await _imageRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Image entity)
        {
            return await _imageRepository.DeleteAsync(entity);
        }
    }
}
