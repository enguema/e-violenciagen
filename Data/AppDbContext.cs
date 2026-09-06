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

    public DbSet<TipoInstitucion> TiposInstitucion => Set<TipoInstitucion>();

    public DbSet<Institucion> Instituciones => Set<Institucion>();

    public DbSet<UnidadOrganizativa> UnidadesOrganizativas =>
        Set<UnidadOrganizativa>();


    // =========================================================
    // TERRITORIO
    // =========================================================

    public DbSet<Provincia> Provincias => Set<Provincia>();

    public DbSet<Distrito> Distritos => Set<Distrito>();

    public DbSet<Barrio> Barrios => Set<Barrio>();


    // =========================================================
    // CATÁLOGOS
    // =========================================================

    public DbSet<TipoViolencia> TiposViolencia => Set<TipoViolencia>();

    public DbSet<EstadoCaso> EstadosCaso => Set<EstadoCaso>();

    public DbSet<TipoActuacion> TiposActuacion => Set<TipoActuacion>();

    public DbSet<TipoDocumentoExpediente> TiposDocumentoExpediente => Set<TipoDocumentoExpediente>();

    // =========================================================
    // PERSONAS Y ACTORES
    // =========================================================

    public DbSet<TipoDocumentoIdentidad> TiposDocumentoIdentidad => Set<TipoDocumentoIdentidad>();

    public DbSet<Persona> Personas => Set<Persona>();

    public DbSet<PersonaInstitucion> PersonasInstituciones => Set<PersonaInstitucion>();

    // =========================================================
    // CASOS
    // =========================================================

    public DbSet<Caso> Casos => Set<Caso>();
    public DbSet<CasoTipoViolencia> CasosTiposViolencia => Set<CasoTipoViolencia>();
    public DbSet<CasoVictima> CasosVictimas => Set<CasoVictima>();
    public DbSet<CasoPresuntoAgresor> CasosPresuntosAgresores => Set<CasoPresuntoAgresor>();

    // =========================================================
    // ACTUACIONES INSTITUCIONALES
    // =========================================================
    public DbSet<Actuacion> Actuaciones =>
        Set<Actuacion>();

    public DbSet<ActuacionParticipante> ActuacionesParticipantes =>
        Set<ActuacionParticipante>();

    // =========================================================
    // EXPEDIENTE ELECTRÓNICO / DOCUMENTOS
    // =========================================================

    public DbSet<DocumentoExpediente> DocumentosExpediente =>
        Set<DocumentoExpediente>();

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

        // =========================================================
        // SECUENCIA PARA EL CÓDIGO ADMINISTRATIVO DEL CASO
        // =========================================================

        /*
         * PostgreSQL será responsable de entregar números
         * consecutivos de forma segura incluso si varios usuarios
         * registran casos simultáneamente.
         */
        modelBuilder
            .HasSequence<long>("CasoCodigoSequence")
            .StartsAt(1)
            .IncrementsBy(1);


        // Aplicamos automáticamente las configuraciones
        // IEntityTypeConfiguration<T>.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}