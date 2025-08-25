namespace Bank.Application.DTOs;

public record EstadoCuentaRequest(int ClienteId, DateTime Desde, DateTime Hasta, string? Format = "json");
public record EstadoCuentaMovimientoDto(int MovimientoId,DateTime Fecha, string Tipo, decimal Valor, decimal Saldo);
public record EstadoCuentaCuentaDto(int CuentaId, string Numero, string Tipo, decimal SaldoInicial, decimal TotalCreditos, decimal TotalDebitos, decimal SaldoFinal, List<EstadoCuentaMovimientoDto> Movimientos);
public record EstadoCuentaResponse(int ClienteId, string ClienteNombre, DateTime Desde, DateTime Hasta, List<EstadoCuentaCuentaDto> Cuentas, decimal TotalCreditos, decimal TotalDebitos, decimal SaldoFinal);