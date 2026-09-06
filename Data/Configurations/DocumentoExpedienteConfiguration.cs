using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_violenciagen.Models;
/// <summary>
/// Define cómo se almacena DocumentoExpediente
/// y sus relaciones en PostgreSQL.
/// </summary>
public class DocumentoExpedienteConfiguration
    : IEntityTypeConfiguration<DocumentoExpediente>
{
    public void Configure(
        EntityTypeBuilder<DocumentoExpediente> builder)
    {
        // =====================================================
        // CLAVE PRIMARIA
        // =====================================================

        builder.HasKey(x => x.Id);


        // =====================================================
        // PROPIEDADES DEL ARCHIVO
        // =====================================================

        builder.Property(x => x.NombreOriginal)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.NombreAlmacenado)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.RutaAlmacenamiento)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(x => x.TipoMime)
            .HasMaxLength(150);

        builder.Property(x => x.Extension)
            .HasMaxLength(20);

        builder.Property(x => x.HashArchivo)
            .HasMaxLength(128);

        builder.Property(x => x.Titulo)
            .HasMaxLength(250);

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);


        // =====================================================
        // CASO
        // =====================================================

        /*
         * Todo documento pertenece obligatoriamente
         * a un caso.
         */
        builder.HasOne(x => x.Caso)
            .WithMany(x => x.Documentos)
            .HasForeignKey(x => x.CasoId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // ACTUACIÓN
        // =====================================================

        /*
         * La actuación es opcional.
         *
         * Si una actuación se elimina, preferimos restringir
         * el borrado mientras existan documentos asociados.
         *
         * Esto evita perder accidentalmente el contexto
         * documental.
         */
        builder.HasOne(x => x.Actuacion)
            .WithMany(x => x.Documentos)
            .HasForeignKey(x => x.ActuacionId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // TIPO DE DOCUMENTO
        // =====================================================

        builder.HasOne(x => x.TipoDocumentoExpediente)
            .WithMany(x => x.Documentos)
            .HasForeignKey(x => x.TipoDocumentoExpedienteId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // ÍNDICES
        // =====================================================

        /*
         * Será muy frecuente consultar todos los documentos
         * pertenecientes a un caso.
         */
        builder.HasIndex(x => x.CasoId);


        /*
         * También habrá consultas de documentos asociados
         * a una actuación.
         */
        builder.HasIndex(x => x.ActuacionId);


        /*
         * Facilita filtros por tipo documental.
         */
        builder.HasIndex(x => x.TipoDocumentoExpedienteId);


        /*
         * El hash puede servir posteriormente para detectar
         * archivos duplicados.
         *
         * No lo hacemos UNIQUE porque dos expedientes podrían
         * legítimamente contener copias del mismo documento.
         */
        builder.HasIndex(x => x.HashArchivo);


        /*
         * Evitamos que dos registros apunten al mismo archivo
         * físico.
         */
        builder.HasIndex(x => x.RutaAlmacenamiento)
            .IsUnique();
    }
}