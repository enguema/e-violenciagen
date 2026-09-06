using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;

/// <summary>
/// Configuración de la relación muchos-a-muchos entre
/// Actuacion y PersonaInstitucion.
/// </summary>
public class ActuacionParticipanteConfiguration
    : IEntityTypeConfiguration<ActuacionParticipante>
{
    public void Configure(
        EntityTypeBuilder<ActuacionParticipante> builder)
    {
        // =====================================================
        // CLAVE PRIMARIA COMPUESTA
        // =====================================================

        builder.HasKey(x => new
        {
            x.ActuacionId,
            x.PersonaInstitucionId
        });


        // =====================================================
        // ACTUACIÓN
        // =====================================================

        builder.HasOne(x => x.Actuacion)
            .WithMany(x => x.Participantes)
            .HasForeignKey(x => x.ActuacionId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // PERSONA / INSTITUCIÓN
        // =====================================================

        builder.HasOne(x => x.PersonaInstitucion)
            .WithMany(x => x.ParticipacionesActuaciones)
            .HasForeignKey(x => x.PersonaInstitucionId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.RolEnActuacion)
            .HasMaxLength(150);


        /*
         * Facilita localizar todas las actuaciones
         * en las que participó un profesional concreto.
         */
        builder.HasIndex(x => x.PersonaInstitucionId);
    }
}