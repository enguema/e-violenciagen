using e_violenciagen.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
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

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            /*
            * Nombre y apellidos son datos propios
            * de nuestra aplicación, no de Identity.
            */
            entity.Property(u => u.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.Apellidos)
                .HasMaxLength(150)
                .IsRequired();


            /*
            * Activo es nuestro control administrativo.
            *
            * Identity ya tiene su propio sistema de bloqueo
            * mediante LockoutEnd y AccessFailedCount.
            */
            entity.Property(u => u.Activo)
                .IsRequired();


            /*
            * La fecha de creación es obligatoria.
            */
            entity.Property(u => u.FechaCreacion)
                .IsRequired();

            // =========================================================
            // INSTITUCIÓN
            // =========================================================

            entity.HasOne(u => u.Institucion)
                .WithMany(i => i.Usuarios)
                .HasForeignKey(u => u.InstitucionId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // UNIDAD ORGANIZATIVA
            // =========================================================

            entity.HasOne(u => u.UnidadOrganizativa)
                .WithMany(uo => uo.Usuarios)
                .HasForeignKey(u => u.UnidadOrganizativaId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // =========================================================
        // IDENTITY - ROL
        // =========================================================

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(r => r.Descripcion)
                .HasMaxLength(250);

            entity.Property(r => r.Activo)
                .IsRequired();

            entity.Property(r => r.EsSistema)
                .IsRequired();
        });

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