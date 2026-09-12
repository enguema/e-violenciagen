using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;

/// <summary>
/// Define restricciones y relaciones de Persona.
/// </summary>
public class PersonaConfiguration
    : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombres)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.Apellidos)
            .IsRequired(false)
            .HasMaxLength(150);


        // =====================================================
        // TIPO DOCUMENTO
        // =====================================================

        builder.HasOne(x => x.TipoDocumentoIdentidad)
            .WithMany(x => x.Personas)
            .HasForeignKey(x => x.TipoDocumentoIdentidadId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // TERRITORIO
        // =====================================================

        builder.HasOne(x => x.Barrio)
            .WithMany()
            .HasForeignKey(x => x.BarrioId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // ÍNDICES
        // =====================================================

        /*
         * Cuando conocemos tipo y número de documento,
         * la combinación debería identificar a una sola persona.
         *
         * Creamos por tanto un índice compuesto único.
         */
        builder.HasIndex(p => p.Nombres);

        builder.HasIndex(p => p.Apellidos);

        builder.HasIndex(p => p.NumeroDocumento);

        
        builder.HasIndex(x => new
        {
            x.TipoDocumentoIdentidadId,
            x.NumeroDocumento
        })
        .IsUnique();
    }
}