using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;
/// <summary>
/// Configura la vinculación entre una persona
/// y una institución.
/// </summary>
public class PersonaInstitucionConfiguration
    : IEntityTypeConfiguration<PersonaInstitucion>
{
    public void Configure(
        EntityTypeBuilder<PersonaInstitucion> builder)
    {
        builder.HasKey(x => x.Id);


        // =====================================================
        // PERSONA
        // =====================================================

        builder.HasOne(x => x.Persona)
            .WithMany(x => x.VinculacionesInstitucionales)
            .HasForeignKey(x => x.PersonaId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // INSTITUCIÓN
        // =====================================================

        builder.HasOne(x => x.Institucion)
            .WithMany()
            .HasForeignKey(x => x.InstitucionId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // UNIDAD ORGANIZATIVA
        // =====================================================

        builder.HasOne(x => x.UnidadOrganizativa)
            .WithMany()
            .HasForeignKey(x => x.UnidadOrganizativaId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.Cargo)
            .HasMaxLength(150);

        builder.Property(x => x.CodigoProfesional)
            .HasMaxLength(100);
    }
}