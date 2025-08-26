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
        var isSqlite = provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
        var isSqlServer = provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

        if (isSqlite)
        {
            modelBuilder.Entity<Persona>(e =>
            {
                e.Property(p => p.PersonaId)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Cliente>(e =>
            {
                e.Property(x => x.ClienteId)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Cuenta>(b =>
            {
                b.HasKey(c => c.CuentaId);
                b.Property(c => c.CuentaId).ValueGeneratedOnAdd();

                 b.HasOne<Cliente>()                                // principal: Cliente
                .WithMany(c => c.Cuentas)                         // si tienes la colección; si no, .WithMany()
                .HasForeignKey(x => x.ClienteId)                  // FK en Cuenta
                .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
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
     public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        if (Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
        {
            var nuevos = ChangeTracker.Entries<Cliente>()
                          .Where(e => e.State == EntityState.Added && e.Entity.ClienteId == 0)
                          .ToList();

            var n = await base.SaveChangesAsync(ct);

            foreach (var e in nuevos)
                e.Entity.ClienteId = e.Entity.PersonaId;
            
            if (nuevos.Count > 0)
                n += await base.SaveChangesAsync(ct);

            return n;
        }

        return await base.SaveChangesAsync(ct);
    }
}