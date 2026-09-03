using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;

/// <summary>
/// Configura la relación muchos-a-muchos explícita
/// entre Caso y TipoViolencia.
/// </summary>
public class CasoTipoViolenciaConfiguration: IEntityTypeConfiguration<CasoTipoViolencia>
{
    public void Configure( EntityTypeBuilder<CasoTipoViolencia> builder)
    {
        // =====================================================
        // CLAVE COMPUESTA
        // =====================================================

        builder.HasKey(x => new
        {
            x.CasoId,
            x.TipoViolenciaId
        });


        // =====================================================
        // CASO
        // =====================================================

        builder.HasOne(x => x.Caso)
            .WithMany(x => x.TiposViolencia)
            .HasForeignKey(x => x.CasoId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // TIPO DE VIOLENCIA
        // =====================================================

        builder.HasOne(x => x.TipoViolencia)
            .WithMany(x => x.Casos)
            .HasForeignKey(x => x.TipoViolenciaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}