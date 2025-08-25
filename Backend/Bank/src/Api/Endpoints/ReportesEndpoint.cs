using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

// PDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Api.Endpoints;

public static class ReportesEndpoints
{
    public static IEndpointRouteBuilder MapReportes(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/reportes");

        g.MapGet("/estado-cuenta", async (
            int clienteId, DateTime desde, DateTime hasta, string? format,
            IClienteRepository cliRepo, ICuentaRepository ctaRepo, IMovimientoRepository movRepo) =>
        {
            var cliente = await cliRepo.GetByIdAsync(clienteId);
            if (cliente is null) return Results.NotFound(new { detail = "Cliente no encontrado." });

            // Cuentas del cliente
            var cuentas = await ctaRepo.ListWithClienteAsync(c => c.ClienteId == clienteId);

            var cuentasDto = new List<EstadoCuentaCuentaDto>();
            decimal totCred = 0, totDeb = 0, saldoFinalCliente = 0;

            foreach (var c in cuentas)
            {
                // Movs en rango
                var movs = await movRepo.ListAsync(m =>
                    m.CuentaId == c.CuentaId && m.Fecha >= desde && m.Fecha <= hasta);

                // Saldo inicial: último saldo ANTES de 'desde' o saldo inicial de cuenta
                var movAntes = (await movRepo.ListAsync(m => m.CuentaId == c.CuentaId && m.Fecha < desde))
                               .OrderBy(m => m.Fecha).LastOrDefault();
                var saldoInicial = movAntes?.Saldo ?? c.Saldo;

                var totalCreditos = movs.Where(m => m.Valor > 0).Sum(m => m.Valor);
                var totalDebitos  = movs.Where(m => m.Valor < 0).Sum(m => Math.Abs(m.Valor));
                var saldoFinal    = saldoInicial + totalCreditos - totalDebitos;

                totCred += totalCreditos;
                totDeb  += totalDebitos;
                saldoFinalCliente += saldoFinal;

                var movDtos = movs.OrderBy(m => m.Fecha)
                    .Select(m => new EstadoCuentaMovimientoDto(m.MovimientoId,m.Fecha, m.TipoMovimiento!, m.Valor, m.Saldo))
                    .ToList();

                cuentasDto.Add(new EstadoCuentaCuentaDto(
                    c.CuentaId, c.NumeroCuenta ?? "", c.TipoCuenta ?? "",
                    saldoInicial, totalCreditos, totalDebitos, saldoFinal, movDtos));
            }

            var json = new EstadoCuentaResponse(
                (int)cliente.PersonaId, cliente.Nombre ?? "", desde, hasta,
                cuentasDto, totCred, totDeb, saldoFinalCliente);

            format = (format ?? "json").ToLowerInvariant();

            if (format == "pdf")
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var bytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.Header().Text($"Estado de Cuenta - {cliente.Nombre}").SemiBold().FontSize(16);
                        page.Content().Column(col =>
                        {
                            col.Item().Text($"Cliente: {cliente.Nombre} (ID {cliente.PersonaId})");
                            col.Item().Text($"Rango: {desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}");
                            col.Item().Text($"Totales - Créditos: {totCred:C2}  Débitos: {totDeb:C2}  Saldo Final: {saldoFinalCliente:C2}")
                                .FontSize(11).Bold();
                            col.Item().LineHorizontal(1);

                            foreach (var c in cuentasDto)
                            {
                                col.Item().Text($"Cuenta {c.Numero} ({c.Tipo})").Bold().FontSize(13);
                                col.Item().Text($"Saldo Inicial: {c.SaldoInicial:C2}  Créditos: {c.TotalCreditos:C2}  Débitos: {c.TotalDebitos:C2}  Saldo Final: {c.SaldoFinal:C2}")
                                    .FontSize(11);
                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(cdef =>
                                    {
                                        cdef.ConstantColumn(90);   // Fecha
                                        cdef.RelativeColumn();      // Tipo
                                        cdef.RelativeColumn();      // Valor
                                        cdef.RelativeColumn();      // Saldo
                                    });
                                    t.Header(h =>
                                    {
                                        h.Cell().Text("Fecha").SemiBold();
                                        h.Cell().Text("Tipo").SemiBold();
                                        h.Cell().Text("Valor").SemiBold();
                                        h.Cell().Text("Saldo").SemiBold();
                                    });
                                    foreach (var m in c.Movimientos)
                                    {
                                        t.Cell().Text(m.Fecha.ToString("yyyy-MM-dd"));
                                        t.Cell().Text(m.Tipo);
                                        t.Cell().Text(m.Valor.ToString("N2"));
                                        t.Cell().Text(m.Saldo.ToString("N2"));
                                    }
                                });
                                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            }
                        });
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Generado ").FontSize(10);
                            x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")).FontSize(10);
                        });
                    });
                }).GeneratePdf();

                var b64 = Convert.ToBase64String(bytes);
                return Results.Ok(new
                {
                    fileName = $"estado-cliente-{cliente.PersonaId}-{desde:yyyyMMdd}-{hasta:yyyyMMdd}.pdf",
                    contentType = "application/pdf",
                    data = b64
                });
            }

            // JSON por defecto
            return Results.Ok(json);
        });

        return app;
    }
}