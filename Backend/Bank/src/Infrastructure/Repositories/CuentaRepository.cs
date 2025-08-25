using System.Linq.Expressions;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

public class CuentaRepository : GenericRepository<Cuenta>, ICuentaRepository
{
    public CuentaRepository(BankingDbContext ctx) : base(ctx) { }
    public async Task<Cuenta?> GetForUpdateAsync(int cuentaId)
        => await _ctx.Cuentas.Include(c => c.Movimientos)
            .FirstOrDefaultAsync(c => c.CuentaId == cuentaId);
    public async Task<Cuenta?> GetByNumeroAsync(string numeroCuenta)
        => await _ctx.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

    public async Task<List<Cuenta>> ListWithClienteAsync(Expression<Func<Cuenta, bool>>? filter = null)
    {
        var q = _ctx.Cuentas
            .AsNoTracking()
            .Include(c => c.Cliente)
            .AsQueryable();

        if (filter != null) q = q.Where(filter);
        return await q.ToListAsync();
    }

}