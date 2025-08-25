using System.Linq.Expressions;
using Bank.Domain.Entities;
namespace Bank.Application.Interfaces;

public interface ICuentaRepository : IGenericRepository<Cuenta>
{
    Task<Cuenta?> GetForUpdateAsync(int cuentaId);
    Task<Cuenta?> GetByNumeroAsync(string numeroCuenta);
    Task<List<Cuenta>> ListWithClienteAsync(Expression<Func<Cuenta, bool>>? filter = null);
}