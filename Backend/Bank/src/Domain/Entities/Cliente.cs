namespace Bank.Domain.Entities;

public class Cliente : Persona
{
    public int? ClienteId { get; set; }
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}