namespace Bank.Application.DTOs;

public record CuentaCreateDto(int ClienteId, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial, bool Estado);
public record CuentaReadDto(int CuentaId, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial, bool Estado, int ClienteId, string ClienteNombre);
public record CuentaUpdateDto(int ClienteId, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial, bool Estado);
public record CuentaListItemDto(int CuentaId, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial, bool Estado, int ClienteId, string ClienteNombre);