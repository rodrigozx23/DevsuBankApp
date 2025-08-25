using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<Cliente?> GetWithCuentasAsync(int clienteId);
}