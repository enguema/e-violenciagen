using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;

/// <summary>
/// Configuración de persistencia de las actuaciones
/// institucionales.
/// </summary>
public class ActuacionConfiguration
    : IEntityTypeConfiguration<Actuacion>
{
    public void Configure(EntityTypeBuilder<Actuacion> builder)
    {
        builder.HasKey(x => x.Id);


        // =====================================================
        // PROPIEDADES
        // =====================================================

        builder.Property(x => x.Titulo)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(x => x.Descripcion)
            .HasMaxLength(5000);

        builder.Property(x => x.Resultado)
            .HasMaxLength(2000);


        // =====================================================
        // CASO
        // =====================================================

        /*
         * Caso 1 ----- N Actuacion
         *
         * Toda actuación debe pertenecer a un caso.
         */
        builder.HasOne(x => x.Caso)
            .WithMany(x => x.Actuaciones)
            .HasForeignKey(x => x.CasoId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // TIPO DE ACTUACIÓN
        // =====================================================

        builder.HasOne(x => x.TipoActuacion)
            .WithMany(x => x.Actuaciones)
            .HasForeignKey(x => x.TipoActuacionId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // INSTITUCIÓN
        // =====================================================

        builder.HasOne(x => x.Institucion)
            .WithMany(x => x.Actuaciones)
            .HasForeignKey(x => x.InstitucionId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // UNIDAD ORGANIZATIVA
        // =====================================================

        builder.HasOne(x => x.UnidadOrganizativa)
            .WithMany(x => x.Actuaciones)
            .HasForeignKey(x => x.UnidadOrganizativaId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // ÍNDICES
        // =====================================================

        /*
         * Será frecuente consultar todas las actuaciones
         * de un caso ordenadas cronológicamente.
         */
        builder.HasIndex(x => new
        {
            x.CasoId,
            x.FechaActuacion
        });


        /*
         * También necesitaremos estadísticas por institución.
         */
        builder.HasIndex(x => x.InstitucionId);


        /*
         * Y filtros por tipo de actuación.
         */
        builder.HasIndex(x => x.TipoActuacionId);
    }
}