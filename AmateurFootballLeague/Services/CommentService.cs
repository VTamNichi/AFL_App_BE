using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public IQueryable<Comment> GetList(params Expression<Func<Comment, object>>[] includes)
        {
            return _commentRepository.GetList(includes);
        }
        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _commentRepository.GetByIdAsync(id);
        }
        public async Task<Comment> AddAsync(Comment entity)
        {
            return await _commentRepository.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(Comment entity)
        {
            return await _commentRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(Comment entity)
        {
            return await _commentRepository.DeleteAsync(entity);
        }
    }
}
