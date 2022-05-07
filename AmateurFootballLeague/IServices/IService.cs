using System.Linq.Expressions;

namespace AmateurFootballLeague.IServices
{
    public interface IService<T, TKey>
    {
        IQueryable<T> GetList(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
