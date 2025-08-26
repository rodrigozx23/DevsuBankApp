using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Exceptions;
using System.Drawing;


namespace Bank.Application.Services;


public class MovimientoService
{
    private readonly ICuentaRepository _cuentas;
    private readonly IMovimientoRepository _movs;
    private readonly IUnitOfWork _uow;
    private const decimal LIMITE_DIARIO_RETIRO = 1000m;

    public MovimientoService(ICuentaRepository cuentas, IMovimientoRepository movs, IUnitOfWork uow)
    {
        _cuentas = cuentas;
        _movs = movs;
        _uow = uow;
    }

    public async Task<MovimientoReadDto> RegistrarAsync(MovimientoCreateDto dto)
    {
        var cuenta = await _cuentas.GetForUpdateAsync(dto.CuentaId)
            ?? throw new DomainException("Cuenta no existe.");

        if (!cuenta.Estado)
            throw new DomainException("Cuenta inactiva.");

        var ahora = dto.Fecha?.ToUniversalTime() ?? DateTime.UtcNow;

        decimal saldoActual = cuenta.Movimientos?.OrderBy(m => m.Fecha).LastOrDefault()?.Saldo
                              ?? cuenta.Saldo;

        var tipo = (dto.TipoMovimiento ?? "").Trim().ToLowerInvariant();
        if (tipo != "credito" && tipo != "debito")
            throw new DomainException("Tipo inválido. Use 'Credito' o 'Debito'.");

        decimal valor = Math.Abs(dto.Valor);
        if (valor <= 0) throw new DomainException("Valor inválido.");

        if (tipo == "debito")
        {
            if (saldoActual <= 0 || saldoActual < valor)
                throw new DomainException("Saldo no disponible");

            var inicioDia = ahora.Date;
            var finDia = inicioDia.AddDays(1);

            var retirosHoy = cuenta.Movimientos?
                .Where(m => m.Fecha >= inicioDia && m.Fecha < finDia && m.Valor < 0)
                .Sum(m => Math.Abs(m.Valor)) ?? 0m;

            if (retirosHoy + valor > LIMITE_DIARIO_RETIRO)
                throw new DomainException("Cupo diario Excedido");
        }

        var valorFirmado = tipo == "debito" ? -valor : valor;
        var nuevoSaldo = saldoActual + valorFirmado;

        var mov = new Movimiento
        {
            CuentaId = cuenta.CuentaId,
            Fecha = ahora,
            TipoMovimiento = tipo == "debito" ? "Debito" : "Credito",
            Valor = valorFirmado,
            Saldo = nuevoSaldo
        };

        await _movs.AddAsync(mov);
        await _uow.SaveChangesAsync();

        cuenta.Saldo = nuevoSaldo;
        await _uow.SaveChangesAsync();

        return new MovimientoReadDto(mov.MovimientoId, mov.Fecha, mov.TipoMovimiento, mov.Valor, mov.Saldo, mov.CuentaId);
    }

    public async Task<object?> ListarAsync(int cuentaId, DateTime? desde, DateTime? hasta)
    {
        var list = await _movs.ListAsync(m =>
            m.CuentaId == cuentaId &&
            (!desde.HasValue || m.Fecha >= desde.Value) &&
            (!hasta.HasValue || m.Fecha <= hasta.Value));

        return list
            .OrderBy(m => m.Fecha)
            .Select(m => new MovimientoReadDto(
                m.MovimientoId, m.Fecha, m.TipoMovimiento!, m.Valor, m.Saldo, m.CuentaId))
            .ToList();
    }
}