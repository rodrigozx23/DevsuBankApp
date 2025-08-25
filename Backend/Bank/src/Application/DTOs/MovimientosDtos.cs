namespace Bank.Application.DTOs;

public record MovimientoCreateDto(int CuentaId, string TipoMovimiento, decimal Valor, DateTime? Fecha = null);
public record MovimientoReadDto(int MovimientoId, DateTime Fecha, string TipoMovimiento, decimal Valor, decimal Saldo, int CuentaId);