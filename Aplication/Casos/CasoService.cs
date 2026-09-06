using System.Data;
using e_violenciagen.Data;
using e_violenciagen.Models;
using e_violenciagen.ViewModels;
using e_violenciagen.ViewModels.Casos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace e_violenciagen.Aplication.Casos;
// <summary>
/// Implementa la lógica de consulta del expediente.
///
/// El Controller no accede directamente al AppDbContext.
/// Toda la obtención y transformación de datos se concentra aquí.
/// </summary>
public class CasoService : ICasoService
{
    private readonly AppDbContext _dbContext;

    public CasoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    // =========================================================
    // LISTADO GENERAL
    // =========================================================

    public async Task<IReadOnlyList<CasoIndexItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Casos
            .AsNoTracking()

            // Mostramos inicialmente únicamente casos activos.
            .Where(c => c.Activo)

            // Ordenamos mostrando primero los expedientes
            // registrados más recientemente.
            .OrderByDescending(c => c.FechaRegistro)

            /*
             * Proyectamos directamente desde PostgreSQL
             * hacia el ViewModel.
             *
             * Esto evita cargar entidades completas que
             * la pantalla Index no necesita.
             */
            .Select(c => new CasoIndexItemViewModel
            {
                Id = c.Id,
                CodigoCaso = c.CodigoCaso,
                FechaRegistro = c.FechaRegistro,
                FechaHecho = c.FechaHecho,

                Estado = c.EstadoCaso.Nombre,

                NumeroVictimas = c.Victimas.Count(),

                NumeroPresuntosAgresores =
                    c.PresuntosAgresores.Count(),

                NumeroActuaciones =
                    c.Actuaciones.Count(a => a.Activo),

                NumeroDocumentos =
                    c.Documentos.Count(d => d.Activo)
            })

            .ToListAsync(cancellationToken);
    }


    // =========================================================
    // EXPEDIENTE COMPLETO
    // =========================================================

    public async Task<CasoDetailsViewModel?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        /*
         * Utilizamos proyección en lugar de una enorme cadena
         * de Include().
         *
         * EF Core genera las consultas necesarias y nosotros
         * obtenemos solamente los datos utilizados por la vista.
         */
        return await _dbContext.Casos
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CasoDetailsViewModel
            {
                Id = c.Id,
                CodigoCaso = c.CodigoCaso,

                Estado = c.EstadoCaso.Nombre,
                CodigoEstado = c.EstadoCaso.Codigo,

                FechaRegistro = c.FechaRegistro,
                FechaHecho = c.FechaHecho,

                Resumen = c.Resumen,
                RelatoInicial = c.RelatoInicial,
                LugarDescripcion = c.LugarDescripcion,

                // Utilizamos navegación desde Barrio.
                Barrio = c.BarrioHecho != null
                    ? c.BarrioHecho.Nombre
                    : null,

                Distrito = c.BarrioHecho != null
                    ? c.BarrioHecho.Distrito.Nombre
                    : null,

                Provincia = c.BarrioHecho != null
                    ? c.BarrioHecho.Distrito.Provincia.Nombre
                    : null,


                // ---------------------------------------------
                // TIPOS DE VIOLENCIA
                // ---------------------------------------------

                TiposViolencia = c.TiposViolencia
                    .Where(x => x.TipoViolencia.Activo)
                    .Select(x => x.TipoViolencia.Nombre)
                    .OrderBy(x => x)
                    .ToList(),


                // ---------------------------------------------
                // VÍCTIMAS
                // ---------------------------------------------

                Victimas = c.Victimas
                    .Select(v => new CasoPersonaViewModel
                    {
                        PersonaId = v.PersonaId,

                        NombreCompleto =
                            v.Persona.Nombres + " " +
                            v.Persona.Apellidos,

                        TipoDocumento =
                            v.Persona.TipoDocumentoIdentidad != null
                                ? v.Persona.TipoDocumentoIdentidad.Nombre
                                : null,

                        NumeroDocumento =
                            v.Persona.NumeroDocumento,

                        FechaNacimiento =
                            v.Persona.FechaNacimiento,

                        Telefono =
                            v.Persona.Telefono,

                        EsPrincipal =
                            v.EsVictimaPrincipal,

                        RequiereProteccion =
                            v.RequiereProteccion,

                        Observaciones =
                            v.Observaciones
                    })
                    .OrderByDescending(v => v.EsPrincipal)
                    .ThenBy(v => v.NombreCompleto)
                    .ToList(),


                // ---------------------------------------------
                // PRESUNTOS AGRESORES
                // ---------------------------------------------

                PresuntosAgresores = c.PresuntosAgresores
                    .Select(a => new CasoPersonaViewModel
                    {
                        PersonaId = a.PersonaId,

                        NombreCompleto =
                            a.Persona.Nombres + " " +
                            a.Persona.Apellidos,

                        TipoDocumento =
                            a.Persona.TipoDocumentoIdentidad != null
                                ? a.Persona.TipoDocumentoIdentidad.Nombre
                                : null,

                        NumeroDocumento =
                            a.Persona.NumeroDocumento,

                        FechaNacimiento =
                            a.Persona.FechaNacimiento,

                        Telefono =
                            a.Persona.Telefono,

                        RelacionConVictima =
                            a.RelacionConVictima,

                        Observaciones =
                            a.Observaciones
                    })
                    .OrderBy(a => a.NombreCompleto)
                    .ToList(),


                // ---------------------------------------------
                // ACTUACIONES
                // ---------------------------------------------

                Actuaciones = c.Actuaciones
                    .Where(a => a.Activo)
                    .OrderByDescending(a => a.FechaActuacion)

                    .Select(a => new CasoActuacionViewModel
                    {
                        Id = a.Id,

                        FechaActuacion =
                            a.FechaActuacion,

                        TipoActuacion =
                            a.TipoActuacion.Nombre,

                        Titulo =
                            a.Titulo,

                        Descripcion =
                            a.Descripcion,

                        Resultado =
                            a.Resultado,

                        Institucion =
                            a.Institucion.Nombre,

                        UnidadOrganizativa =
                            a.UnidadOrganizativa != null
                                ? a.UnidadOrganizativa.Nombre
                                : null,

                        NumeroParticipantes =
                            a.Participantes.Count(),

                        NumeroDocumentos =
                            a.Documentos.Count(d => d.Activo)
                    })
                    .ToList(),


                // ---------------------------------------------
                // DOCUMENTOS
                // ---------------------------------------------

                Documentos = c.Documentos
                    .Where(d => d.Activo)
                    .OrderByDescending(d => d.FechaIncorporacion)

                    .Select(d => new CasoDocumentoViewModel
                    {
                        Id = d.Id,

                        NombreOriginal =
                            d.NombreOriginal,

                        TipoDocumento =
                            d.TipoDocumentoExpediente.Nombre,

                        FechaDocumento =
                            d.FechaDocumento,

                        FechaIncorporacion =
                            d.FechaIncorporacion,

                        Titulo =
                            d.Titulo,

                        TipoMime =
                            d.TipoMime,

                        TamanoBytes =
                            d.TamanoBytes,

                        EsConfidencial =
                            d.EsConfidencial,

                        ActuacionId =
                            d.ActuacionId,

                        ActuacionTitulo =
                            d.Actuacion != null
                                ? d.Actuacion.Titulo
                                : null
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
    }


    // =========================================================
    // ACTUACIONES
    // =========================================================

    public async Task<IReadOnlyList<CasoActuacionViewModel>>
        GetActuacionesAsync(
            Guid casoId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Actuaciones
            .AsNoTracking()
            .Where(a =>
                a.CasoId == casoId &&
                a.Activo)

            .OrderByDescending(a => a.FechaActuacion)

            .Select(a => new CasoActuacionViewModel
            {
                Id = a.Id,
                FechaActuacion = a.FechaActuacion,
                TipoActuacion = a.TipoActuacion.Nombre,
                Titulo = a.Titulo,
                Descripcion = a.Descripcion,
                Resultado = a.Resultado,
                Institucion = a.Institucion.Nombre,

                UnidadOrganizativa =
                    a.UnidadOrganizativa != null
                        ? a.UnidadOrganizativa.Nombre
                        : null,

                NumeroParticipantes =
                    a.Participantes.Count(),

                NumeroDocumentos =
                    a.Documentos.Count(d => d.Activo)
            })
            .ToListAsync(cancellationToken);
    }


    // =========================================================
    // DOCUMENTOS
    // =========================================================

    public async Task<IReadOnlyList<CasoDocumentoViewModel>>
        GetDocumentosAsync(
            Guid casoId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.DocumentosExpediente
            .AsNoTracking()
            .Where(d =>
                d.CasoId == casoId &&
                d.Activo)

            .OrderByDescending(d => d.FechaIncorporacion)

            .Select(d => new CasoDocumentoViewModel
            {
                Id = d.Id,
                NombreOriginal = d.NombreOriginal,
                TipoDocumento =
                    d.TipoDocumentoExpediente.Nombre,

                FechaDocumento =
                    d.FechaDocumento,

                FechaIncorporacion =
                    d.FechaIncorporacion,

                Titulo =
                    d.Titulo,

                TipoMime =
                    d.TipoMime,

                TamanoBytes =
                    d.TamanoBytes,

                EsConfidencial =
                    d.EsConfidencial,

                ActuacionId =
                    d.ActuacionId,

                ActuacionTitulo =
                    d.Actuacion != null
                        ? d.Actuacion.Titulo
                        : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CasoCreateViewModel> GetCreateFormAsync(CancellationToken cancellationToken = default)
    {
        var model = new CasoCreateViewModel
        {
            TiposDocumento = await _dbContext.TiposDocumentoIdentidad
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new OpcionCatalogoViewModel
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToListAsync(cancellationToken),

            Provincias = await _dbContext.Provincias
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new OpcionCatalogoViewModel
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToListAsync(cancellationToken),

            TiposViolencia = await _dbContext.TiposViolencia
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new OpcionCatalogoViewModel
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToListAsync(cancellationToken)
        };

        return model;
    }

    public async Task<IReadOnlyList<OpcionCatalogoViewModel>> GetDistritosAsync(Guid provinciaId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Distritos
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.ProvinciaId == provinciaId)
            .OrderBy(x => x.Nombre)
            .Select(x => new OpcionCatalogoViewModel
            {
                Id = x.Id,
                Nombre = x.Nombre
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<OpcionCatalogoViewModel>>
        GetBarriosAsync(
            Guid distritoId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Barrios
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.DistritoId == distritoId)
            .OrderBy(x => x.Nombre)
            .Select(x => new OpcionCatalogoViewModel
            {
                Id = x.Id,
                Nombre = x.Nombre
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CasoCreateResultViewModel> CreateAsync(CasoCreateViewModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);


        // =========================================================
        // VALIDACIONES PREVIAS
        // =========================================================

        if (model.TiposViolenciaIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Debe seleccionar al menos un tipo de violencia.");
        }


        // No aceptamos fechas futuras del hecho.
        if (model.FechaHecho.HasValue)
        {
            DateOnly hoy =
                DateOnly.FromDateTime(DateTime.UtcNow);

            if (model.FechaHecho.Value > hoy)
            {
                throw new InvalidOperationException(
                    "La fecha del hecho no puede estar en el futuro.");
            }
        }


        /*
         * Al tener habilitada una estrategia de reintentos en Npgsql,
         * ejecutamos toda la transacción mediante la estrategia
         * proporcionada por EF Core.
         *
         * Esto evita el conocido error:
         *
         * "The configured execution strategy does not support
         * user-initiated transactions".
         */
        var executionStrategy =
            _dbContext.Database.CreateExecutionStrategy();


        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _dbContext.Database
                    .BeginTransactionAsync(cancellationToken);

            try
            {
                // =================================================
                // ESTADO INICIAL
                // =================================================

                var estadoRegistrado =
                    await _dbContext.EstadosCaso
                        .SingleOrDefaultAsync(x =>
                            x.Codigo == "REGISTRADO" &&
                            x.Activo,
                            cancellationToken);

                if (estadoRegistrado is null)
                {
                    throw new InvalidOperationException(
                        "No está configurado el estado inicial REGISTRADO.");
                }


                // =================================================
                // TIPOS DE VIOLENCIA
                // =================================================

                List<Guid> tiposIds =
                    model.TiposViolenciaIds
                        .Distinct()
                        .ToList();


                List<Guid> tiposValidos =
                    await _dbContext.TiposViolencia
                        .Where(x =>
                            tiposIds.Contains(x.Id) &&
                            x.Activo)
                        .Select(x => x.Id)
                        .ToListAsync(cancellationToken);


                if (tiposValidos.Count != tiposIds.Count)
                {
                    throw new InvalidOperationException(
                        "Uno o varios tipos de violencia seleccionados no son válidos.");
                }


                // =================================================
                // VALIDAR TERRITORIO
                // =================================================

                if (model.BarrioHechoId.HasValue)
                {
                    bool barrioValido =
                        await _dbContext.Barrios
                            .AnyAsync(x =>
                                x.Id == model.BarrioHechoId.Value &&
                                x.Activo,
                                cancellationToken);

                    if (!barrioValido)
                    {
                        throw new InvalidOperationException(
                            "El barrio seleccionado no es válido.");
                    }
                }


                // =================================================
                // VÍCTIMA
                // =================================================

                Persona victima =
                    await ResolverPersonaAsync(
                        model.Victima,
                        cancellationToken);


                // =================================================
                // PRESUNTO AGRESOR
                // =================================================

                Persona? presuntoAgresor = null;

                if (model.IncluirPresuntoAgresor)
                {
                    presuntoAgresor =
                        await ResolverPersonaAsync(
                            model.PresuntoAgresor,
                            cancellationToken);


                    /*
                     * Si ambas personas ya existían, podemos comparar
                     * directamente sus identificadores.
                     */
                    if (victima.Id == presuntoAgresor.Id)
                    {
                        throw new InvalidOperationException(
                            "Una persona no puede figurar simultáneamente " +
                            "como víctima y presunto agresor en el mismo caso.");
                    }
                }


                // =================================================
                // CÓDIGO ADMINISTRATIVO
                // =================================================

                string codigoCaso =
                    await GenerarCodigoCasoAsync(cancellationToken);


                // =================================================
                // CREAR CASO
                // =================================================

                var caso = new Caso
                {
                    CodigoCaso = codigoCaso,

                    FechaRegistro = DateTime.UtcNow,

                    FechaHecho = model.FechaHecho,

                    EstadoCasoId = estadoRegistrado.Id,

                    BarrioHechoId = model.BarrioHechoId,

                    LugarDescripcion =
                        string.IsNullOrWhiteSpace(model.LugarDescripcion)
                            ? null
                            : model.LugarDescripcion.Trim(),

                    Resumen =
                        string.IsNullOrWhiteSpace(model.Resumen)
                            ? null
                            : model.Resumen.Trim(),

                    RelatoInicial =
                        string.IsNullOrWhiteSpace(model.RelatoInicial)
                            ? null
                            : model.RelatoInicial.Trim()
                };


                // =================================================
                // ASOCIAR TIPOS DE VIOLENCIA
                // =================================================

                foreach (Guid tipoId in tiposValidos)
                {
                    caso.TiposViolencia.Add(
                        new CasoTipoViolencia
                        {
                            CasoId = caso.Id,
                            TipoViolenciaId = tipoId
                        });
                }


                // =================================================
                // ASOCIAR VÍCTIMA PRINCIPAL
                // =================================================

                caso.Victimas.Add(
                    new CasoVictima
                    {
                        CasoId = caso.Id,
                        PersonaId = victima.Id,

                        Persona = victima,

                        EsVictimaPrincipal = true,

                        RequiereProteccion =
                            model.VictimaRequiereProteccion,

                        Observaciones =
                            string.IsNullOrWhiteSpace(
                                model.ObservacionesVictima)
                                ? null
                                : model.ObservacionesVictima.Trim(),

                        FechaVinculacion = DateTime.UtcNow
                    });


                // =================================================
                // ASOCIAR PRESUNTO AGRESOR
                // =================================================

                if (presuntoAgresor is not null)
                {
                    caso.PresuntosAgresores.Add(
                        new CasoPresuntoAgresor
                        {
                            CasoId = caso.Id,
                            PersonaId = presuntoAgresor.Id,

                            Persona = presuntoAgresor,

                            RelacionConVictima =
                                string.IsNullOrWhiteSpace(
                                    model.RelacionConVictima)
                                    ? null
                                    : model.RelacionConVictima.Trim(),

                            FechaVinculacion = DateTime.UtcNow
                        });
                }


                _dbContext.Casos.Add(caso);

                await _dbContext.SaveChangesAsync(
                    cancellationToken);


                await transaction.CommitAsync(
                    cancellationToken);


                return new CasoCreateResultViewModel
                {
                    Id = caso.Id,
                    CodigoCaso = caso.CodigoCaso
                };
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        });
    }

    

    //========== Métodos privados ==========
    private async Task<string> GenerarCodigoCasoAsync(CancellationToken cancellationToken)
    {
        /*
         * Obtenemos el siguiente número directamente
         * desde la secuencia de PostgreSQL.
         */
        var connection = _dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        command.CommandText =
            """SELECT nextval('"CasoCodigoSequence"');""";

        /*
         * Si estamos dentro de una transacción EF Core,
         * asociamos también el comando ADO.NET.
         */
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            command.Transaction =
                _dbContext.Database
                    .CurrentTransaction
                    .GetDbTransaction();
        }

        object? result =
            await command.ExecuteScalarAsync(cancellationToken);

        long numero = Convert.ToInt64(result);

        int anio = DateTime.UtcNow.Year;

        return $"VG-{anio}-{numero:D6}";
    }

    //Evitará dublicados de persona; ejmpl: juan nguema ondó (puebe haber varios con este nombre exacto)
    private async Task<Persona> ResolverPersonaAsync(PersonaRegistroViewModel model, CancellationToken cancellationToken)
    {
        /*
         * Normalizamos el número para reducir duplicaciones
         * causadas por espacios o diferencias de mayúsculas.
         */
        string? numeroDocumento =
            string.IsNullOrWhiteSpace(model.NumeroDocumento)
                ? null
                : model.NumeroDocumento
                    .Trim()
                    .ToUpperInvariant();


        // ---------------------------------------------------------
        // SI HAY DOCUMENTO, INTENTAMOS LOCALIZAR LA PERSONA
        // ---------------------------------------------------------

        if (model.TipoDocumentoIdentidadId.HasValue &&
            numeroDocumento is not null)
        {
            bool tipoDocumentoExiste =
                await _dbContext.TiposDocumentoIdentidad
                    .AnyAsync(x =>
                        x.Id == model.TipoDocumentoIdentidadId.Value &&
                        x.Activo,
                        cancellationToken);

            if (!tipoDocumentoExiste)
            {
                throw new InvalidOperationException(
                    "El tipo de documento seleccionado no es válido.");
            }


            Persona? existente =
                await _dbContext.Personas
                    .SingleOrDefaultAsync(x =>
                        x.TipoDocumentoIdentidadId ==
                            model.TipoDocumentoIdentidadId.Value &&
                        x.NumeroDocumento == numeroDocumento,
                        cancellationToken);

            if (existente is not null)
            {
                return existente;
            }
        }


        // ---------------------------------------------------------
        // SI NO EXISTE, CREAMOS UNA NUEVA PERSONA
        // ---------------------------------------------------------

        var persona = new Persona
        {
            Nombres = model.Nombres!.Trim(),
            Apellidos = model.Apellidos!.Trim(),

            TipoDocumentoIdentidadId =
                model.TipoDocumentoIdentidadId,

            NumeroDocumento =
                numeroDocumento,

            FechaNacimiento =
                model.FechaNacimiento,

            Sexo =
                string.IsNullOrWhiteSpace(model.Sexo)
                    ? null
                    : model.Sexo.Trim(),

            Telefono =
                string.IsNullOrWhiteSpace(model.Telefono)
                    ? null
                    : model.Telefono.Trim(),

            Email =
                string.IsNullOrWhiteSpace(model.Email)
                    ? null
                    : model.Email.Trim()
        };

        _dbContext.Personas.Add(persona);

        return persona;
    }


}