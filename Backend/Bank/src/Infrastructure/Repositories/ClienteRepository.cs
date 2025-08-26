using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(BankingDbContext ctx) : base(ctx) { }
    public async Task<Cliente?> GetWithCuentasAsync(int clienteId)
        => await _ctx.Clientes.Include(c => c.Cuentas)
            .FirstOrDefaultAsync(c => c.PersonaId == clienteId);
    public Task AddAsync(Cliente cli, CancellationToken ct = default)
        => _ctx.Clientes.AddAsync(cli, ct).AsTask(); 
}