using System;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure;

namespace Bank.Api.Endpoints;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes");

        // LISTAR con búsqueda (q) + paginación
        group.MapGet("", async (IClienteRepository repo, string? q, int page = 1, int pageSize = 20) =>
        {
            q = (q ?? string.Empty).Trim().ToLowerInvariant();

            var all = await repo.ListAsync(c =>
                string.IsNullOrEmpty(q) ||
                (!string.IsNullOrEmpty(c.Nombre) && c.Nombre!.ToLower().Contains(q)) ||
                (!string.IsNullOrEmpty(c.Identificacion) && c.Identificacion!.ToLower().Contains(q)));

            var total = all.Count;
            var data = all
                .OrderBy(c => c.PersonaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClienteListItemDto(
                    (int)c.PersonaId,
                    c.Nombre ?? string.Empty,
                    c.Identificacion ?? string.Empty,
                    c.Estado))
                .ToList();

            return Results.Ok(new { total, data });
        });

        // OBTENER por id
        group.MapGet("/{id:int}", async (int id, IClienteRepository repo) =>
        {
            var c = await repo.GetWithCuentasAsync(id);
            return c is null
                ? Results.NotFound(new { detail = "Cliente no encontrado." })
                : Results.Ok(new ClienteReadDto(c.PersonaId, c.Nombre!, c.Identificacion!, c.Genero!, c.Edad!, c.Direccion! ,c.Telefono!,c.Contrasena,c.Estado ));
        });

        // CREAR
        group.MapPost("", async (ClienteCreateDto dto, IClienteRepository repo, IUnitOfWork uow) =>
        {
            var cli = new Cliente
            {
                Nombre = dto.Nombre,
                Genero = dto.Genero,
                Edad = dto.Edad,
                Identificacion = dto.Identificacion,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Contrasena = dto.Contrasena,
                Estado = dto.Estado
            };

            await repo.AddAsync(cli);
            await uow.SaveChangesAsync();
            var read = new ClienteReadDto(cli.PersonaId, cli.Nombre!, cli.Identificacion!, cli.Genero!, cli.Edad!, cli.Direccion! ,cli.Telefono!,cli.Contrasena,cli.Estado );
            return Results.Created($"/api/clientes/{cli.PersonaId}", read);
        });

        // ACTUALIZAR
        group.MapPut("/{id:int}", async (int id, ClienteUpdateDto dto, IClienteRepository repo, IUnitOfWork uow) =>
        {
            var db = await repo.GetByIdAsync(id);
            if (db is null) return Results.NotFound(new { detail = "Cliente no encontrado." });

            db.Nombre = dto.Nombre;
            db.Genero = dto.Genero;
            db.Edad = (int)dto.Edad;
            db.Identificacion = dto.Identificacion;
            db.Direccion = dto.Direccion;
            db.Telefono = dto.Telefono;
            db.Contrasena = dto.Contrasena;
            db.Estado = dto.Estado;

            repo.Update(db);
            await uow.SaveChangesAsync();
            return Results.NoContent();
        });

        // ELIMINAR
        group.MapDelete("/{id:int}", async (int id, IClienteRepository repo, IUnitOfWork uow) =>
        {
            var db = await repo.GetByIdAsync(id);
            if (db is null) return Results.NotFound(new { detail = "Cliente no encontrado." });

            repo.Delete(db);
            await uow.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }
}