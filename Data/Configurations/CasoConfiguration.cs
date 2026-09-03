using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;
/// <summary>
/// Configuración de persistencia de la entidad Caso.
/// </summary>
public class CasoConfiguration
    : IEntityTypeConfiguration<Caso>
{
    public void Configure(EntityTypeBuilder<Caso> builder)
    {
        // =====================================================
        // CLAVE PRIMARIA
        // =====================================================

        builder.HasKey(x => x.Id);


        // =====================================================
        // CÓDIGO DEL CASO
        // =====================================================

        builder.Property(x => x.CodigoCaso)
            .IsRequired(false)
            .HasMaxLength(30);

        /*
         * Dos casos no pueden utilizar el mismo código
         * administrativo.
         */
        builder.HasIndex(x => x.CodigoCaso)
            .IsUnique();


        // =====================================================
        // TEXTOS
        // =====================================================

        builder.Property(x => x.Resumen)
            .HasMaxLength(500);

        builder.Property(x => x.LugarDescripcion)
            .HasMaxLength(500);

        builder.Property(x => x.RelatoInicial)
            .HasMaxLength(5000);


        // =====================================================
        // ESTADO
        // =====================================================

        /*
         * EstadoCaso 1 -------- N Caso
         *
         * Todo caso debe tener un estado.
         */
        builder.HasOne(x => x.EstadoCaso)
            .WithMany(x => x.Casos)
            .HasForeignKey(x => x.EstadoCasoId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // TERRITORIO
        // =====================================================

        /*
         * Barrio 1 -------- N Caso
         *
         * BarrioHechoId es nullable porque puede no conocerse
         * inicialmente el lugar exacto del hecho.
         */
        builder.HasOne(x => x.BarrioHecho)
            .WithMany()
            .HasForeignKey(x => x.BarrioHechoId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // ÍNDICES ÚTILES
        // =====================================================

        /*
         * Será habitual buscar o generar estadísticas
         * por fecha de registro.
         */
        builder.HasIndex(x => x.FechaRegistro);

        /*
         * También serán frecuentes los filtros por estado.
         */
        builder.HasIndex(x => x.EstadoCasoId);

        /*
         * Y las estadísticas territoriales por lugar del hecho.
         */
        builder.HasIndex(x => x.BarrioHechoId);
    }
}