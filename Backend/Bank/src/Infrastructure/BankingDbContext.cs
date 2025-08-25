using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bank.Infrastructure;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);
            
        var provider = Database.ProviderName ?? "";
        var isSqlite   = provider.Contains("Sqlite",   StringComparison.OrdinalIgnoreCase);
        var isSqlServer= provider.Contains("SqlServer",StringComparison.OrdinalIgnoreCase);

        if (isSqlite)
        {
            modelBuilder.Entity<Persona>(e =>
            {
                e.Property(p => p.PersonaId)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<TestSequentialIntGenerator>();
            });

            modelBuilder.Entity<Cliente>(e =>
            {
                e.Property(x => x.ClienteId)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<TestSequentialIntGenerator>();
            });
        }
        else if (isSqlServer)
        {

            modelBuilder.Entity<Cliente>(e =>
            {
                e.Property(x => x.ClienteId)
                 .UseIdentityColumn()
                 .ValueGeneratedOnAdd();

                e.Property(c => c.ClienteId).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                e.Property(c => c.ClienteId).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
            modelBuilder.Entity<Persona>(e =>
            {
                e.Property(x => x.PersonaId)
                 .UseIdentityColumn()
                 .ValueGeneratedOnAdd();
            });
        }
    }
}