using Bank.Application.DTOs;
using Bank.Application.Services;

namespace Bank.Api.Endpoints;

public static class MovimientosEndpoints
{
    public static IEndpointRouteBuilder MapMovimientos(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movimientos");
            group.MapPost("", async (MovimientoCreateDto dto, MovimientoService svc) =>
        {
            var mov = await svc.RegistrarAsync(dto);
            return Results.Created($"/api/movimientos/{mov.MovimientoId}", mov);
        });

        // Listar por cuenta y fechas
        group.MapGet("", async (int cuentaId, DateTime? desde, DateTime? hasta, MovimientoService svc) =>
        {
            var data = await svc.ListarAsync(cuentaId, desde, hasta);
            return Results.Ok(data);
        });
        return app;
    }
}