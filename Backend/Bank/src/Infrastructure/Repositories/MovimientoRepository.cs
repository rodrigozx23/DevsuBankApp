using System.Linq.Expressions;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

public class MovimientoRepository : GenericRepository<Movimiento>, IMovimientoRepository
{
    public MovimientoRepository(BankingDbContext ctx) : base(ctx) {}
    public async Task<List<Movimiento>> ListAsync(Expression<Func<Movimiento, bool>>? filter)
    {
        var q = _ctx.Movimientos.AsQueryable();
        if (filter != null) q = q.Where(filter);
        return await q.ToListAsync();
    }
}