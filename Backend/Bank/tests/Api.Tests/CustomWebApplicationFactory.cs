using Bank.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _conn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Quitar el DbContext original
            var toRemove = services.Where(s => s.ServiceType == typeof(DbContextOptions<BankingDbContext>)).ToList();
            foreach (var d in toRemove) services.Remove(d);

            // SQLite en memoria (mantén la conexión abierta)
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            services.AddDbContext<BankingDbContext>(opt => opt.UseSqlite(_conn));

            // Construir y crear esquema
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
            db.Database.EnsureCreated();

            // Seed mínimo (coincide con tu modelo TPT)
            var cli = new Bank.Domain.Entities.Cliente
            {
                Nombre = "Cliente Test",
                Identificacion = "CLI-T1",
                Estado = true,
                Genero = "M",
                Edad = 30,
                Direccion = "Calle 1",
                Telefono = "999",
                Contrasena = "x"
            };
            db.Clientes.Add(cli);  // EF hará INSERT Personas -> INSERT Clientes
            db.SaveChanges();

            db.Cuentas.Add(new Bank.Domain.Entities.Cuenta
            {
                ClienteId = (int)cli.PersonaId,   // PK compartida
                NumeroCuenta = "T-0001",
                TipoCuenta = "Ahorros",
                Saldo = 100m,
                Estado = true
            });
            db.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}