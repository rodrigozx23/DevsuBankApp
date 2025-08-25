namespace Bank.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}