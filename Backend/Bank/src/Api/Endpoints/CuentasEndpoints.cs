using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;

namespace Bank.Api.Endpoints;

public static class CuentasEndpoints
{
    public static IEndpointRouteBuilder MapCuentas(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cuentas");
        
        // LISTAR + búsqueda por número o nombre cliente
        group.MapGet("", async (ICuentaRepository repo, IClienteRepository cRepo, string? q, int page = 1, int pageSize = 20) =>
        {
            q = (q ?? "").Trim().ToLowerInvariant();
              var all = await repo.ListWithClienteAsync(c => string.IsNullOrEmpty(q) ||
              (!string.IsNullOrEmpty(c.NumeroCuenta) && c.NumeroCuenta.ToLower().Contains(q)) ||
                (c.Cliente != null && c.Cliente.Nombre != null && c.Cliente.Nombre.ToLower().Contains(q))
            );

            var total = all.Count;
            var data = all
                .OrderBy(c => c.CuentaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CuentaListItemDto(
                    c.CuentaId,
                    c.NumeroCuenta!,
                    c.TipoCuenta!,
                    c.Saldo,
                    c.Estado,
                    c.ClienteId,
                     c.Cliente?.Nombre ?? ""))
                .ToList();

            return Results.Ok(new { total, data });
        });

        // LISTAR por cliente
        group.MapGet("/by-cliente/{clienteId:int}", async (int clienteId, ICuentaRepository repo) =>
        {
            var list = await repo.ListAsync(c => c.ClienteId == clienteId);
            var data = list.Select(c => new CuentaListItemDto(
                c.CuentaId, c.NumeroCuenta!, c.TipoCuenta!, c.Saldo, c.Estado, c.ClienteId, c.Cliente?.Nombre ?? "")).ToList();
            return Results.Ok(data);
        });

        // DETALLE
        group.MapGet("/{id:int}", async (int id, ICuentaRepository repo) =>
        {
            var c = await repo.GetByIdAsync(id);
            return c is null
                ? Results.NotFound(new { detail = "Cuenta no encontrada." })
                : Results.Ok(new CuentaReadDto(
                    c.CuentaId, c.NumeroCuenta!, c.TipoCuenta!, c.Saldo, c.Estado, c.ClienteId, c.Cliente?.Nombre ?? ""));
        });

        // CREAR
        group.MapPost("", async (CuentaCreateDto dto, ICuentaRepository repo, IUnitOfWork uow) =>
        {
            var cta = new Bank.Domain.Entities.Cuenta {
                ClienteId = dto.ClienteId, NumeroCuenta = dto.NumeroCuenta, TipoCuenta = dto.TipoCuenta,
                Saldo = dto.SaldoInicial, Estado = dto.Estado
            };
            await repo.AddAsync(cta);
            await uow.SaveChangesAsync();
            return Results.Created($"/api/cuentas/{cta.CuentaId}",
                new CuentaListItemDto(cta.CuentaId, cta.NumeroCuenta!, cta.TipoCuenta!, cta.Saldo, cta.Estado, cta.ClienteId, ""));
        });

        // ACTUALIZAR
        group.MapPut("/{id:int}", async (int id, CuentaUpdateDto dto, ICuentaRepository repo, IUnitOfWork uow) =>
        {
            var db = await repo.GetByIdAsync(id);
            if (db is null) return Results.NotFound(new { detail = "Cuenta no encontrada." });

            db.ClienteId = dto.ClienteId;
            db.NumeroCuenta = dto.NumeroCuenta;
            db.TipoCuenta = dto.TipoCuenta;
            db.Saldo = dto.SaldoInicial;
            db.Estado = dto.Estado;

            repo.Update(db);
            await uow.SaveChangesAsync();
            return Results.NoContent();
        });

        // ELIMINAR
        group.MapDelete("/{id:int}", async (int id, ICuentaRepository repo, IUnitOfWork uow) =>
        {
            var db = await repo.GetByIdAsync(id);
            if (db is null) return Results.NotFound(new { detail = "Cuenta no encontrada." });

            repo.Delete(db);
            await uow.SaveChangesAsync();
            return Results.NoContent();
        });
        return app;
    }
}