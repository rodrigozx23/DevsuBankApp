using Bank.Application.Interfaces;

namespace Bank.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly BankingDbContext _ctx;
    public UnitOfWork(BankingDbContext ctx) => _ctx = ctx;
    public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
    public Task<int> SaveChangesAsync(CancellationToken ct) => _ctx.SaveChangesAsync(ct);
}