using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations;

public class CuentaConfig : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
        builder.ToTable("Cuentas");
        builder.HasKey(c => c.CuentaId);
        builder.HasIndex(c => c.NumeroCuenta).IsUnique();
        builder.Property(c => c.TipoCuenta).HasMaxLength(30);
        builder.Property(c => c.Saldo).HasColumnType("decimal(18,2)");
    }
}