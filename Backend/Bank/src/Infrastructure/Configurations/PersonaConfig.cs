using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Bank.Infrastructure.Configurations;


public class PersonaConfig : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("Personas");
        builder.UseTptMappingStrategy();  
        builder.HasKey(p => p.PersonaId);

        builder.Property(p => p.PersonaId)
               .ValueGeneratedOnAdd();
               
        builder.Property(p => p.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Identificacion)
            .HasMaxLength(20)
            .IsRequired();
    }
}