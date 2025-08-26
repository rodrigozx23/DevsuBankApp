using Bank.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _keepAlive; // mantiene viva la DB en memoria

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1) Quitar el DbContext original (SQL Server)
            var toRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<BankingDbContext>))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            // 2) Crear UNA conexión en memoria y mantenerla abierta
            _keepAlive = new SqliteConnection("DataSource=:memory:");
            _keepAlive.Open();

            // 3) Re-registrar DbContext usando ESA conexión
            services.AddDbContext<BankingDbContext>(opt =>
            {
                opt.UseSqlite(_keepAlive);
            });

            // 4) Construir provider y crear esquema
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BankingDbContext>();

            db.Database.EnsureCreated(); // ← tablas listas en la misma conexión

            // 5) (Opcional) Semillas para pruebas
            // db.Add(new Cliente { ... });
            // db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _keepAlive?.Dispose(); // cierra la conexión al terminar TODO el factory
    }
}
