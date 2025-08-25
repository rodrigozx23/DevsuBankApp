using Bank.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly BankingDbContext _ctx;
    public GenericRepository(BankingDbContext ctx) => _ctx = ctx;

    public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> ListAsync(System.Linq.Expressions.Expression<Func<T,bool>>? filter=null)
    {
        IQueryable<T> q = _ctx.Set<T>();
        if (filter != null) q = q.Where(filter);
        return await q.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);
    public void Update(T entity) => _ctx.Set<T>().Update(entity);
    public void Delete(T entity) => _ctx.Set<T>().Remove(entity);
}