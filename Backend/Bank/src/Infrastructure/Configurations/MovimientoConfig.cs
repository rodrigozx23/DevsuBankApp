using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations;

public class MovimientoConfig : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> builder)
    {
        builder.ToTable("Movimientos");
        builder.HasKey(m => m.MovimientoId);
        builder.Property(m => m.Valor).HasColumnType("decimal(18,2)");
        builder.Property(m => m.Saldo).HasColumnType("decimal(18,2)");
        builder.HasOne(m => m.Cuenta)
            .WithMany(c => c.Movimientos)
            .HasForeignKey(m => m.CuentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}