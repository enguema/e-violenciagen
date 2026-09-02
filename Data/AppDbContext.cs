using e_violenciagen.Models;
using Microsoft.EntityFrameworkCore;

namespace e_violenciagen.Data;

/// <summary>
/// Contexto principal de acceso a datos de SIGEVIG.
///
/// Todas las entidades persistentes del sistema se irán
/// incorporando progresivamente a este contexto.
///
/// En esta primera fase todavía no tenemos entidades
/// del dominio porque únicamente estamos preparando
/// la infraestructura de acceso a PostgreSQL.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // =========================================================
    // MODELO INSTITUCIONAL
    // =========================================================

    public DbSet<TipoInstitucion> TiposInstitucion =>
        Set<TipoInstitucion>();

    public DbSet<Institucion> Instituciones =>
        Set<Institucion>();

    public DbSet<UnidadOrganizativa> UnidadesOrganizativas =>
        Set<UnidadOrganizativa>();


    // =========================================================
    // TERRITORIO
    // =========================================================

    public DbSet<Provincia> Provincias =>
        Set<Provincia>();

    public DbSet<Distrito> Distritos =>
        Set<Distrito>();

    public DbSet<Barrio> Barrios =>
        Set<Barrio>();


    // =========================================================
    // CATÁLOGOS
    // =========================================================

    public DbSet<TipoViolencia> TiposViolencia =>
        Set<TipoViolencia>();

    public DbSet<EstadoCaso> EstadosCaso =>
        Set<EstadoCaso>();

    public DbSet<TipoActuacion> TiposActuacion =>
        Set<TipoActuacion>();

    public DbSet<TipoDocumentoExpediente> TiposDocumentoExpediente =>
        Set<TipoDocumentoExpediente>();

    /// <summary>
    /// Aquí configuraremos progresivamente las entidades
    /// mediante Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /*
         * Más adelante podremos aplicar configuraciones
         * automáticamente desde este ensamblado.
         *
         * Ejemplo futuro:
         *
         * modelBuilder.ApplyConfigurationsFromAssembly(
         *     typeof(AppDbContext).Assembly);
         */
    }
}