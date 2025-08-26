using Api.Endpoints;
using Bank.Api.Endpoints;
using Bank.Api.Middleware;
using Bank.Application.Interfaces;
using Bank.Application.Services;
using Bank.Infrastructure;
using Bank.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("ng", p => p
        .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// EF Core
var cs = builder.Configuration.GetConnectionString("Default") ??
"Server=sqlserver,1433;Database=BankDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True";

builder.Services.AddDbContext<BankingDbContext>(opt => opt.UseSqlServer(cs));

// DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IMovimientoRepository, MovimientoRepository>();
builder.Services.AddScoped<MovimientoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("ng");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapClientes();
app.MapCuentas();
app.MapMovimientos();
app.MapReportes();

if (Environment.GetEnvironmentVariable("AUTO_MIGRATE") == "true")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }