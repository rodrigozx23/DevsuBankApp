namespace Bank.Domain.Entities;

public class Movimiento
{
    public int MovimientoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Saldo{ get; set; }
    public int CuentaId { get; set; }
    public Cuenta? Cuenta { get; set; }
}