using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;
/// <summary>
/// Configuración de la relación entre Caso y Persona
/// cuando la persona figura como presunto agresor.
/// </summary>
public class CasoPresuntoAgresorConfiguration
    : IEntityTypeConfiguration<CasoPresuntoAgresor>
{
    public void Configure(
        EntityTypeBuilder<CasoPresuntoAgresor> builder)
    {
        // =====================================================
        // CLAVE PRIMARIA COMPUESTA
        // =====================================================

        /*
         * Una persona no puede aparecer dos veces como
         * presunto agresor dentro del mismo caso.
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
            .WithMany(x => x.PresuntosAgresores)
            .HasForeignKey(x => x.CasoId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // PERSONA
        // =====================================================

        builder.HasOne(x => x.Persona)
            .WithMany(x => x.CasosComoPresuntoAgresor)
            .HasForeignKey(x => x.PersonaId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.RelacionConVictima)
            .HasMaxLength(100);

        builder.Property(x => x.Observaciones)
            .HasMaxLength(1000);


        builder.HasIndex(x => x.PersonaId);
    }
}