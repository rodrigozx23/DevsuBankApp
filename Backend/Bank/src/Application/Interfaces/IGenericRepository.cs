using System.Linq.Expressions;

namespace Bank.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? filter = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}