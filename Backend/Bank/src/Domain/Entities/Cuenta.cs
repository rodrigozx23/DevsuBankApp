namespace Bank.Domain.Entities;

public class Cuenta
{
    public int CuentaId { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = "Ahorros";
    public decimal Saldo { get; set; }
    public bool Estado { get; set; } = true;


    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }


    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}