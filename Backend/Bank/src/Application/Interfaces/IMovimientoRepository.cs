using System.Linq.Expressions;
using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IMovimientoRepository : IGenericRepository<Movimiento>
{     
    Task<List<Movimiento>> ListAsync(Expression<Func<Movimiento,bool>>? filter = null);
}