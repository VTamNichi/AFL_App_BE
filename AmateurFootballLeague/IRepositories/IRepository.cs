using System.Linq.Expressions;

namespace AmateurFootballLeague.IRepositories
{
    public interface IRepository<T, TKey>
    {
        IQueryable<T> GetList(params Expression<Func<T, object>>[] predicate);
        Task<T> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        void DeleteRange(Expression<Func<T, bool>> predicate);
        int CountAsync(params Expression<Func<T, object>>[] predicate);
    }
}