using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Bank.Infrastructure.Configurations;

public class ClienteConfig : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.Property(c => c.Contrasena)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(c => c.Estado)
               .HasDefaultValue(true)
               .IsRequired();

        //builder.Property(c => c.ClienteId)
        //.UseIdentityColumn()
        //.ValueGeneratedOnAdd();

        // Ensure EF never tries to send it before/after save
        builder.Property(c => c.ClienteId).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(c => c.ClienteId).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}