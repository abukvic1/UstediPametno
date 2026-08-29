using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
namespace UstediPametno.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
