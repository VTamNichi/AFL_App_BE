using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AmateurFootballLeague.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected AmateurFootballLeagueContext _dbContext;
        public Repository(AmateurFootballLeagueContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<T> GetList(params Expression<Func<T, object>>[] predicate)
        {
            try
            {
                IQueryable<T> queryList = _dbContext.Set<T>().AsNoTracking();
                foreach (Expression<Func<T, object>> expression in predicate)
                {
                    queryList = queryList.Include(expression);
                }
                return queryList;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entities: {nameof(GetList)} because: {ex.Message}");
            }
        }
        public async Task<T> GetByIdAsync(TKey id)
        {
            try
            {
                return await _dbContext.FindAsync<T>(id);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entity: {nameof(GetByIdAsync)} because: {ex.Message}");
            }
        }
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }
        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                _dbContext.Update(entity);
                int updatedEntries = await _dbContext.SaveChangesAsync();

                return updatedEntries > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }
        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                _dbContext.Set<T>().Remove(entity);
                int updatedEntries = await _dbContext.SaveChangesAsync();

                return updatedEntries > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be deleted: {ex.Message}");
            }
        }
        public void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> lstRemove = _dbContext.Set<T>().Where(predicate);
            _dbContext.RemoveRange(lstRemove);
        }

        public int CountAsync(params Expression<Func<T, object>>[] predicate)
        {
            try
            {
                IQueryable<T> queryList = _dbContext.Set<T>().AsNoTracking();
                foreach (var expression in predicate)
                {
                    queryList = queryList.Include(expression);
                }
                return queryList.Count();
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Couldn't retrieve entities: {nameof(GetList)} because: {ex.Message}");
            }
        }

    }
}
