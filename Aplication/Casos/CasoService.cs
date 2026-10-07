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

    //agregar victimas
    public async Task AgregarVictimaAsync(
    Guid casoId,
    Guid personaId,
    CancellationToken cancellationToken = default)
    {
        /*
         * 1. Comprobamos que el caso realmente exista.
         */
        bool existeCaso = await _dbContext.Casos
            .AnyAsync(
                c => c.Id == casoId,
                cancellationToken);

        if (!existeCaso)
        {
            throw new KeyNotFoundException("El caso indicado no existe.");
        }

        /*
         * 2. Comprobamos que la persona seleccionada exista.
         *
         * IMPORTANTE:
         * No creamos una Persona nueva.
         * Estamos vinculando una Persona que ya existe
         * en el sistema.
         */
        bool existePersona = await _dbContext.Personas
            .AnyAsync(
                p => p.Id == personaId,
                cancellationToken);

        if (!existePersona)
        {
            throw new InvalidOperationException(
                "La persona seleccionada no existe.");
        }

        /*
        * NUEVA REGLA:
        * La misma persona no puede ser víctima y presunto agresor
        * en el mismo caso.
        */
        await ValidarQueNoExistaConflictoDeRolesAsync(casoId, personaId, seAgregaComoVictima: true, cancellationToken);

        bool yaEsPresuntoAgresor = await _dbContext.CasosPresuntosAgresores
            .AnyAsync(
                x => x.CasoId == casoId && x.PersonaId == personaId,
                cancellationToken);

        if (yaEsPresuntoAgresor)
        {
            throw new InvalidOperationException(
                "La persona seleccionada ya figura como presunto agresor en este caso y no puede registrarse también como víctima.");
        }

        /*
         * 3. Impedimos que la misma persona se agregue
         * varias veces como víctima del mismo caso.
         * y QUE NO PUEDE SER VICTIMA Y PRESUNTO AGRESOR AL MISMO TIEMPO
         */
        bool yaEsVictima = await _dbContext.CasosVictimas
            .AnyAsync(
                cv =>
                    cv.CasoId == casoId &&
                    cv.PersonaId == personaId,
                cancellationToken);

        if (yaEsVictima)
        {
            throw new InvalidOperationException(
                "La persona seleccionada ya está registrada como víctima de este caso.");
        }

        /*
         * 4. Creamos únicamente la relación.
         *
         * La Persona ya existe y no debe duplicarse.
         */
        var casoVictima = new CasoVictima
        {
            CasoId = casoId,
            PersonaId = personaId
        };

        _dbContext.CasosVictimas.Add(casoVictima);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    //Agregar presunto agresor
    public async Task AgregarPresuntoAgresorAsync(
    Guid casoId,
    Guid personaId,
    CancellationToken cancellationToken = default)
    {
        /*
         * 1. Verificamos que el expediente exista.
         */
        bool existeCaso = await _dbContext.Casos
            .AnyAsync(
                c => c.Id == casoId,
                cancellationToken);

        if (!existeCaso)
        {
            throw new KeyNotFoundException(
                "El caso indicado no existe.");
        }

        /*
         * 2. Verificamos que la persona exista.
         *
         * No creamos una Persona nueva.
         * Solo vinculamos una persona existente al expediente.
         */
        bool existePersona = await _dbContext.Personas
            .AnyAsync(
                p => p.Id == personaId,
                cancellationToken);

        if (!existePersona)
        {
            throw new InvalidOperationException(
                "La persona seleccionada no existe.");
        }

        /*
        * NUEVA REGLA:
        * La misma persona no puede ser víctima y presunto agresor
        * en el mismo caso.
        */
        await ValidarQueNoExistaConflictoDeRolesAsync(casoId, personaId, seAgregaComoVictima: false, cancellationToken);

        /*
         * 3. Evitamos duplicados.
         *
         * Una persona no debe aparecer dos veces como
         * presunto agresor dentro del mismo expediente.
         */
        bool yaEsPresuntoAgresor =
            await _dbContext.CasosPresuntosAgresores
                .AnyAsync(
                    x =>
                        x.CasoId == casoId &&
                        x.PersonaId == personaId,
                    cancellationToken);

        if (yaEsPresuntoAgresor)
        {
            throw new InvalidOperationException(
                "La persona seleccionada ya está registrada como presunto agresor de este caso.");
        }

        /*
         * 4. Creamos únicamente la relación.
         */
        var relacion = new CasoPresuntoAgresor
        {
            CasoId = casoId,
            PersonaId = personaId
        };

        _dbContext.CasosPresuntosAgresores.Add(relacion);

        await _dbContext.SaveChangesAsync(cancellationToken);
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
        GetBarriosAsync(Guid distritoId, CancellationToken cancellationToken = default)
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

        // ---------------------------------------------------------
        // VÍCTIMA PRINCIPAL
        // ---------------------------------------------------------

        if (!model.VictimaPersonaId.HasValue)
        {
            throw new InvalidOperationException(
                "Debe seleccionar una víctima principal.");
        }

        // ---------------------------------------------------------
        // FECHA DEL HECHO
        // ---------------------------------------------------------

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
         * Validamos que la víctima principal y el presunto agresor
         * no sean la misma persona.
         */

        if (model.VictimaPersonaId.HasValue &&
            model.PresuntoAgresorPersonaId.HasValue &&
            model.VictimaPersonaId.Value == model.PresuntoAgresorPersonaId.Value)
        {
            throw new InvalidOperationException(
                "La misma persona no puede registrarse como víctima y como presunto agresor dentro del mismo caso.");
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

                /*if (model.BarrioHechoId.HasValue)
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
                }*/
                if (model.BarrioHechoId.HasValue)
                {
                    var barrio =
                        await _dbContext.Barrios
                            .AsNoTracking()
                            .Where(x =>
                                x.Id == model.BarrioHechoId.Value &&
                                x.Activo)
                            .Select(x => new
                            {
                                x.Id,
                                x.DistritoId,
                                ProvinciaId =
                                    x.Distrito.ProvinciaId
                            })
                            .SingleOrDefaultAsync(
                                cancellationToken);


                    if (barrio is null)
                    {
                        throw new InvalidOperationException(
                            "El barrio seleccionado no es válido.");
                    }


                    /*
                     * Aunque los Select de la interfaz son dependientes,
                     * no confiamos únicamente en el navegador.
                     *
                     * También validamos la jerarquía territorial
                     * en el servidor.
                     */
                    if (model.DistritoId.HasValue &&
                        barrio.DistritoId != model.DistritoId.Value)
                    {
                        throw new InvalidOperationException(
                            "El barrio seleccionado no pertenece al distrito indicado.");
                    }


                    if (model.ProvinciaId.HasValue &&
                        barrio.ProvinciaId != model.ProvinciaId.Value)
                    {
                        throw new InvalidOperationException(
                            "El distrito seleccionado no pertenece a la provincia indicada.");
                    }
                }


                // =================================================
                // VÍCTIMA
                // =================================================

                /*Persona victima =
                    await ResolverPersonaAsync(
                        model.Victima,
                        cancellationToken);
                if (!model.VictimaPersonaId.HasValue)
                {
                    throw new InvalidOperationException(
                        "Debe seleccionar una víctima principal.");
                }

                Persona? victima =
                    await _dbContext.Personas
                        .SingleOrDefaultAsync(
                            p =>
                                p.Id == model.VictimaPersonaId.Value &&
                                p.Activo,
                            cancellationToken);

                if (victima is null)
                {
                    throw new InvalidOperationException(
                        "La víctima seleccionada no existe o no está activa.");
                }*/

                Persona? victima =
                await _dbContext.Personas
                    .SingleOrDefaultAsync(
                        p =>
                            p.Id == model.VictimaPersonaId.Value &&
                            p.Activo,
                        cancellationToken);

                if (victima is null)
                {
                    throw new InvalidOperationException(
                        "La víctima seleccionada no existe o no está activa.");
                }


                // =================================================
                // PRESUNTO AGRESOR
                // =================================================

                Persona? presuntoAgresor = null;

                if (model.IncluirPresuntoAgresor)
                {
                    if (!model.PresuntoAgresorPersonaId.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Debe seleccionar el presunto agresor.");
                    }

                    presuntoAgresor =
                        await _dbContext.Personas
                            .SingleOrDefaultAsync(
                                p =>
                                    p.Id == model.PresuntoAgresorPersonaId.Value &&
                                    p.Activo,
                                cancellationToken);

                    if (presuntoAgresor is null)
                    {
                        throw new InvalidOperationException(
                            "El presunto agresor seleccionado no existe o no está activo.");
                    }

                    // Evitamos que la misma persona ocupe ambos roles
                    // dentro del mismo expediente.
                    if (victima.Id == presuntoAgresor.Id)
                    {
                        throw new InvalidOperationException(
                            "Una misma persona no puede ser Victima y Presunto Agresor en el mismo expediente.");
                    }
                }

                //Persona? presuntoAgresor = null;

                /*if (model.IncluirPresuntoAgresor)
                {
                    presuntoAgresor =
                        await ResolverPersonaAsync(
                            model.PresuntoAgresor,
                            cancellationToken);*/


                /*
                 * Si ambas personas ya existían, podemos comparar
                 * directamente sus identificadores.
                 */
                /*if (victima.Id == presuntoAgresor.Id)
                {
                    throw new InvalidOperationException(
                        "Una persona no puede figurar simultáneamente " +
                        "como víctima y presunto agresor en el mismo caso.");
                }*/
                /*}*/


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

                /*caso.Victimas.Add(
                    new CasoVictima
                    {
                        CasoId = caso.Id,
                        PersonaId = model.VictimaPersonaId ?? Guid.NewGuid(),

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
                    });*/

                caso.Victimas.Add(new CasoVictima
                {
                    CasoId = caso.Id,

                    // Persona previamente seleccionada mediante
                    // _PersonaSelector.cshtml.
                    PersonaId = victima.Id,

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

                // =================================================
                // PERSISTENCIA
                // =================================================

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

    //====== Vincular - Desvincular personas a un caso ======
    public async Task DesvincularVictimaAsync(
        Guid casoId,
        Guid personaId,
        CancellationToken cancellationToken = default)
    {
        /*
         * 1. Comprobamos que el caso exista.
         */
        bool existeCaso = await _dbContext.Casos
            .AnyAsync(
                c => c.Id == casoId,
                cancellationToken);

        if (!existeCaso)
        {
            throw new KeyNotFoundException(
                "El caso indicado no existe.");
        }

        /*
         * 2. Buscamos específicamente la relación
         * entre el caso y la víctima.
         *
         * IMPORTANTE:
         * No buscamos la Persona para eliminarla.
         */
        var relacion = await _dbContext.CasosVictimas
            .FirstOrDefaultAsync(
                x =>
                    x.CasoId == casoId &&
                    x.PersonaId == personaId,
                cancellationToken);

        if (relacion is null)
        {
            throw new InvalidOperationException(
                "La persona indicada no está registrada como víctima de este caso.");
        }

        /*
         * 3. El expediente debe conservar al menos
         * una víctima.
         */
        int numeroVictimas = await _dbContext.CasosVictimas
            .CountAsync(
                x => x.CasoId == casoId,
                cancellationToken);

        if (numeroVictimas <= 1)
        {
            throw new InvalidOperationException(
                "No se puede desvincular esta víctima porque el caso debe conservar al menos una víctima asociada.");
        }

        /*
         * 4. Eliminamos únicamente la relación.
         *
         * La Persona permanece intacta en el sistema
         * y puede seguir vinculada a otros casos.
         */
        _dbContext.CasosVictimas.Remove(relacion);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DesvincularPresuntoAgresorAsync(
    Guid casoId,
    Guid personaId,
    CancellationToken cancellationToken = default)
    {
        /*
         * 1. Comprobamos que el caso exista.
         */
        bool existeCaso = await _dbContext.Casos
            .AnyAsync(
                c => c.Id == casoId,
                cancellationToken);

        if (!existeCaso)
        {
            throw new KeyNotFoundException(
                "El caso indicado no existe.");
        }

        /*
         * 2. Buscamos exclusivamente la relación.
         */
        var relacion = await _dbContext.CasosPresuntosAgresores
            .FirstOrDefaultAsync(
                x =>
                    x.CasoId == casoId &&
                    x.PersonaId == personaId,
                cancellationToken);

        if (relacion is null)
        {
            throw new InvalidOperationException(
                "La persona indicada no está registrada como presunto agresor de este caso.");
        }

        /*
         * 3. Eliminamos la vinculación.
         *
         * La Persona NO se elimina.
         */
        _dbContext.CasosPresuntosAgresores.Remove(relacion);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    //========== Métodos privados ==========

    // Evita que una misma persona pueda figurar simultáneamente
    // como víctima y presunto agresor en un mismo caso.
    private async Task ValidarQueNoExistaConflictoDeRolesAsync(
    Guid casoId,
    Guid personaId,
    bool seAgregaComoVictima,
    CancellationToken cancellationToken = default)
    {
        if (seAgregaComoVictima)
        {
            bool yaEsPresuntoAgresor = await _dbContext.CasosPresuntosAgresores
                .AnyAsync(
                    x => x.CasoId == casoId && x.PersonaId == personaId,
                    cancellationToken);

            if (yaEsPresuntoAgresor)
            {
                throw new InvalidOperationException(
                    "La persona seleccionada ya figura como presunto agresor en este caso y no puede registrarse también como víctima.");
            }
        }
        else
        {
            bool yaEsVictima = await _dbContext.CasosVictimas
                .AnyAsync(
                    x => x.CasoId == casoId && x.PersonaId == personaId,
                    cancellationToken);

            if (yaEsVictima)
            {
                throw new InvalidOperationException(
                    "La persona seleccionada ya figura como víctima en este caso y no puede registrarse también como presunto agresor.");
            }
        }
    }
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