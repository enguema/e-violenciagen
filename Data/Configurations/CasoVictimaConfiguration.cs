using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;
/// <summary>
/// Configuración de la relación entre Caso y Persona
/// cuando la persona participa como víctima.
/// </summary>
public class CasoVictimaConfiguration
    : IEntityTypeConfiguration<CasoVictima>
{
    public void Configure(
        EntityTypeBuilder<CasoVictima> builder)
    {
        // =====================================================
        // CLAVE PRIMARIA COMPUESTA
        // =====================================================

        /*
         * Una misma persona no puede aparecer dos veces
         * como víctima dentro del mismo caso.
         */
        builder.HasKey(x => new
        {
            x.CasoId,
            x.PersonaId
        });


        // =====================================================
        // CASO
        // =====================================================

        builder.HasOne(x => x.Caso)
            .WithMany(x => x.Victimas)
            .HasForeignKey(x => x.CasoId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // PERSONA
        // =====================================================

        builder.HasOne(x => x.Persona)
            .WithMany(x => x.CasosComoVictima)
            .HasForeignKey(x => x.PersonaId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.Observaciones)
            .HasMaxLength(1000);


        // Índice útil para localizar rápidamente
        // todos los casos relacionados con una persona.
        builder.HasIndex(x => x.PersonaId);
    }
}