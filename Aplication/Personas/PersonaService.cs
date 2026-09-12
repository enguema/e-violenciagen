using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using e_violenciagen.Dtos.Common;
using e_violenciagen.Dtos.Personas;
using e_violenciagen.Models;

using e_violenciagen.ViewModels.Common;
using e_violenciagen.ViewModels.Personas;
using e_violenciagen.Data;

namespace e_violenciagen.Aplication.Personas;

/// <summary>
/// Implementa la lógica de aplicación relacionada con Persona.
///
/// Responsabilidades principales:
/// - Consultar personas.
/// - Crear personas.
/// - Actualizar personas.
/// - Eliminar personas.
/// - Gestionar fotografías.
/// - Atender el DataTable serverSide.
///
/// El controlador no debe acceder directamente al DbContext.
/// </summary>
public class PersonaService : IPersonaService
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PersonaService> _logger;

    // =========================================================
    // CONFIGURACIÓN DE FOTOGRAFÍAS
    // =========================================================

    /// <summary>
    /// Tamaño máximo permitido para una fotografía:
    /// 5 MB.
    /// </summary>
    private const long MaxPhotoSize = 5 * 1024 * 1024;

    /// <summary>
    /// Tipos MIME que admitiremos como fotografía.
    ///
    /// Deliberadamente no admitimos SVG porque un SVG puede
    /// contener contenido activo y no es necesario para
    /// fotografías de personas.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string>
        AllowedPhotoTypes =
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/webp"] = ".webp"
            };


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public PersonaService(AppDbContext dbContext, IWebHostEnvironment environment, ILogger<PersonaService> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }


    // =========================================================
    // DATATABLE SERVER SIDE
    // =========================================================

    public async Task<DataTableResponse<PersonaListItemDto>> GetDataTableAsync(DataTableRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        /*
         * Nunca confiamos ciegamente en los valores enviados
         * por el navegador.
         */

        int start = Math.Max(request.Start, 0);

        int pageSize = request.Length switch
        {
            <= 0 => 10,
            > 100 => 100,
            _ => request.Length
        };


        // =====================================================
        // CONSULTA BASE
        // =====================================================

        /*
         * AsNoTracking:
         *
         * El listado es de solo lectura.
         * EF Core no necesita realizar seguimiento de cambios
         * sobre estas entidades.
         *
         * Esto reduce memoria y trabajo innecesario.
         */

        IQueryable<Persona> query = _dbContext.Personas
            .AsNoTracking();


        // =====================================================
        // TOTAL SIN FILTRO
        // =====================================================

        int recordsTotal = await query.CountAsync(cancellationToken);


        // =====================================================
        // BÚSQUEDA GLOBAL
        // =====================================================

        string? search = request.SearchValue?.Trim();

        if (!string.IsNullOrWhiteSpace(search))
        {
            /*
             * ILike es específico de PostgreSQL y permite
             * realizar búsquedas case-insensitive.
             *
             * La consulta continúa ejecutándose en PostgreSQL.
             */
            string pattern = $"%{search}%";

            query = query.Where(p =>
                EF.Functions.ILike(p.Nombres, pattern) ||

                EF.Functions.ILike(p.Apellidos, pattern) ||
                EF.Functions.ILike(p.Nombres + " " + p.Apellidos, pattern) ||

                (
                    p.NumeroDocumento != null &&
                    EF.Functions.ILike(
                        p.NumeroDocumento,
                        pattern)
                ) ||

                (
                    p.Telefono != null &&
                    EF.Functions.ILike(
                        p.Telefono,
                        pattern)
                ) ||

                (
                    p.Email != null &&
                    EF.Functions.ILike(
                        p.Email,
                        pattern)
                ));
        }


        // =====================================================
        // TOTAL DESPUÉS DEL FILTRO
        // =====================================================

        //int recordsFiltered = await query.CountAsync(cancellationToken); --- Se ha mejorado abajo
        int recordsFiltered;

        if (string.IsNullOrWhiteSpace(search))
        {
            /*
             * Sin búsqueda sabemos que ambos valores
             * son exactamente iguales.
             *
             * Evitamos un segundo COUNT innecesario.
             */
            recordsFiltered = recordsTotal;
        }
        else
        {
            recordsFiltered =
                await query.CountAsync(
                    cancellationToken);
        }


        // =====================================================
        // ORDENACIÓN
        // =====================================================

        bool descending = string.Equals(
                request.OrderDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        /*
         * Mapeamos nosotros mismos los índices enviados
         * por DataTables.
         *
         * Nunca utilizamos directamente un nombre de columna
         * enviado por el navegador para construir SQL.
         *
         * La distribución prevista para el DataTable será:
         *
         * 0 -> Foto
         * 1 -> Persona
         * 2 -> Tipo documento
         * 3 -> Número documento
         * 4 -> Teléfono
         * 5 -> Email
         * 6 -> Barrio
         * 7 -> Acciones
         *
         * Foto y Acciones no son columnas ordenables.
         */

        query = request.OrderColumn switch
        {
            1 => descending
                ? query
                    .OrderByDescending(p => p.Apellidos)
                    .ThenByDescending(p => p.Nombres)
                : query
                    .OrderBy(p => p.Apellidos)
                    .ThenBy(p => p.Nombres),

            2 => descending
                ? query.OrderByDescending(p =>
                    p.TipoDocumentoIdentidad != null
                        ? p.TipoDocumentoIdentidad.Nombre
                        : string.Empty)
                : query.OrderBy(p =>
                    p.TipoDocumentoIdentidad != null
                        ? p.TipoDocumentoIdentidad.Nombre
                        : string.Empty),

            3 => descending
                ? query.OrderByDescending(
                    p => p.NumeroDocumento)
                : query.OrderBy(
                    p => p.NumeroDocumento),

            4 => descending
                ? query.OrderByDescending(
                    p => p.Telefono)
                : query.OrderBy(
                    p => p.Telefono),

            5 => descending
                ? query.OrderByDescending(
                    p => p.Email)
                : query.OrderBy(
                    p => p.Email),

            6 => descending
                ? query.OrderByDescending(p =>
                    p.Barrio != null
                        ? p.Barrio.Nombre
                        : string.Empty)
                : query.OrderBy(p =>
                    p.Barrio != null
                        ? p.Barrio.Nombre
                        : string.Empty),

            /*
             * Orden por defecto.
             *
             * También se aplica cuando DataTables intenta
             * ordenar Foto o Acciones.
             */
            _ => query
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
        };


        // =====================================================
        // PAGINACIÓN + PROYECCIÓN
        // =====================================================

        /*
         * Este punto es crítico.
         *
         * Skip y Take se ejecutan ANTES de ToListAsync().
         *
         * PostgreSQL solamente devuelve la página solicitada.
         */

        List<PersonaListItemDto> data = await query
            .Skip(start)
            .Take(pageSize)
            .Select(p => new PersonaListItemDto
            {
                Id = p.Id,

                Nombres = p.Nombres,

                Apellidos = p.Apellidos,

                NombreCompleto =
                    p.Nombres + " " + p.Apellidos,

                TipoDocumento =
                    p.TipoDocumentoIdentidad != null
                        ? p.TipoDocumentoIdentidad.Nombre
                        : null,

                NumeroDocumento =
                    p.NumeroDocumento,

                Telefono =
                    p.Telefono,

                Email =
                    p.Email,

                Barrio =
                    p.Barrio != null
                        ? p.Barrio.Nombre
                        : null,

                RutaFoto =
                    p.RutaFoto
            })
            .ToListAsync(cancellationToken);


        return new DataTableResponse<PersonaListItemDto>
        {
            Draw = request.Draw,
            RecordsTotal = recordsTotal,
            RecordsFiltered = recordsFiltered,
            Data = data
        };
    }


    // =========================================================
    // DETAILS
    // =========================================================

    public async Task<PersonaDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        /*
         * No utilizamos Include().
         *
         * Proyectamos directamente al DTO y EF Core genera
         * únicamente la consulta necesaria.
         */

        return await _dbContext.Personas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PersonaDetailsDto
            {
                Id = p.Id,

                // =============================================
                // IDENTIFICACIÓN
                // =============================================

                TipoDocumentoIdentidadId =
                    p.TipoDocumentoIdentidadId,

                TipoDocumento =
                    p.TipoDocumentoIdentidad != null
                        ? p.TipoDocumentoIdentidad.Nombre
                        : null,

                NumeroDocumento =
                    p.NumeroDocumento,


                // =============================================
                // DATOS PERSONALES
                // =============================================

                Nombres =
                    p.Nombres,

                Apellidos =
                    p.Apellidos,

                NombreCompleto =
                    p.Nombres + " " + p.Apellidos,

                FechaNacimiento =
                    p.FechaNacimiento,

                Sexo =
                    p.Sexo,

                RutaFoto =
                    p.RutaFoto,


                // =============================================
                // CONTACTO
                // =============================================

                Telefono =
                    p.Telefono,

                Email =
                    p.Email,


                // =============================================
                // DIRECCIÓN
                // =============================================

                Direccion =
                    p.Direccion,

                BarrioId =
                    p.BarrioId,

                Barrio =
                    p.Barrio != null
                        ? p.Barrio.Nombre
                        : null,

                Distrito =
                    p.Barrio != null &&
                    p.Barrio.Distrito != null
                        ? p.Barrio.Distrito.Nombre
                        : null,

                Provincia =
                    p.Barrio != null &&
                    p.Barrio.Distrito != null &&
                    p.Barrio.Distrito.Provincia != null
                        ? p.Barrio.Distrito.Provincia.Nombre
                        : null,


                // =============================================
                // INFORMACIÓN RELACIONADA
                // =============================================

                NumeroCasosComoVictima =
                    p.CasosComoVictima.Count(),

                NumeroCasosComoPresuntoAgresor =
                    p.CasosComoPresuntoAgresor.Count(),

                NumeroVinculacionesInstitucionales =
                    p.VinculacionesInstitucionales.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    // =========================================================
    // OBTENER PARA EDITAR
    // =========================================================

    public async Task<PersonaFormViewModel?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Personas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PersonaFormViewModel
            {
                Id = p.Id,

                TipoDocumentoIdentidadId =
                    p.TipoDocumentoIdentidadId,

                NumeroDocumento =
                    p.NumeroDocumento,

                Nombres =
                    p.Nombres,

                Apellidos =
                    p.Apellidos,

                FechaNacimiento =
                    p.FechaNacimiento,

                Sexo =
                    p.Sexo,

                Telefono =
                    p.Telefono,

                Email =
                    p.Email,

                Direccion =
                    p.Direccion,

                BarrioId =
                    p.BarrioId,

                /*
                 * No intentamos convertir RutaFoto en IFormFile.
                 *
                 * Foto queda null porque solamente representa
                 * archivos nuevos enviados por el navegador.
                 */
                RutaFotoActual =
                    p.RutaFoto
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    // =========================================================
    // CREAR PERSONA
    // =========================================================

    public async Task<Guid> CreateAsync(PersonaFormViewModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        Normalizar(model);

        await ValidarReferenciasAsync(
            model,
            cancellationToken);

        string? nuevaRutaFoto = null;

        try
        {
            // La fotografía es opcional.
            if (model.Foto is not null &&
                model.Foto.Length > 0)
            {
                nuevaRutaFoto = await SavePhotoAsync(
                    model.Foto,
                    cancellationToken);
            }

            var persona = new Persona
            {
                TipoDocumentoIdentidadId =
                    model.TipoDocumentoIdentidadId,

                NumeroDocumento =
                    model.NumeroDocumento,

                Nombres =
                    model.Nombres,

                Apellidos =
                    model.Apellidos,

                FechaNacimiento =
                    model.FechaNacimiento,

                Sexo =
                    model.Sexo,

                Telefono =
                    model.Telefono,

                Email =
                    model.Email,

                Direccion =
                    model.Direccion,

                BarrioId =
                    model.BarrioId,

                RutaFoto =
                    nuevaRutaFoto
            };

            _dbContext.Personas.Add(persona);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return persona.Id;
        }
        catch
        {
            /*
             * Si conseguimos escribir la foto en disco pero
             * posteriormente PostgreSQL falla, eliminamos
             * el archivo para no dejar fotografías huérfanas.
             */

            if (nuevaRutaFoto is not null)
            {
                DeletePhotoFile(nuevaRutaFoto);
            }

            throw;
        }
    }


    // =========================================================
    // ACTUALIZAR PERSONA
    // =========================================================

    public async Task<bool> UpdateAsync(Guid id, PersonaFormViewModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        Persona? persona = await _dbContext.Personas
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (persona is null)
        {
            return false;
        }

        Normalizar(model);

        await ValidarReferenciasAsync(
            model,
            cancellationToken);

        string? fotoAnterior = persona.RutaFoto;

        string? nuevaRutaFoto = null;

        bool eliminarFotoAnterior = false;

        try
        {
            // =================================================
            // GESTIÓN DE FOTOGRAFÍA
            // =================================================

            if (model.Foto is not null &&
                model.Foto.Length > 0)
            {
                /*
                 * Primero guardamos la nueva fotografía.
                 *
                 * La anterior NO se borra todavía.
                 */
                nuevaRutaFoto = await SavePhotoAsync(
                    model.Foto,
                    cancellationToken);

                persona.RutaFoto = nuevaRutaFoto;

                eliminarFotoAnterior =
                    !string.IsNullOrWhiteSpace(fotoAnterior);
            }
            else if (model.EliminarFoto)
            {
                /*
                 * Marcamos la foto como eliminada en BD,
                 * pero el archivo físico solamente se borrará
                 * después de SaveChangesAsync().
                 */
                persona.RutaFoto = null;

                eliminarFotoAnterior =
                    !string.IsNullOrWhiteSpace(fotoAnterior);
            }


            // =================================================
            // ACTUALIZACIÓN DE DATOS
            // =================================================

            persona.TipoDocumentoIdentidadId =
                model.TipoDocumentoIdentidadId;

            persona.NumeroDocumento =
                model.NumeroDocumento;

            persona.Nombres =
                model.Nombres;

            persona.Apellidos =
                model.Apellidos;

            persona.FechaNacimiento =
                model.FechaNacimiento;

            persona.Sexo =
                model.Sexo;

            persona.Telefono =
                model.Telefono;

            persona.Email =
                model.Email;

            persona.Direccion =
                model.Direccion;

            persona.BarrioId =
                model.BarrioId;


            await _dbContext.SaveChangesAsync(
                cancellationToken);


            // =================================================
            // LIMPIEZA DEL ARCHIVO ANTERIOR
            // =================================================

            /*
             * Llegados aquí PostgreSQL ya ha confirmado
             * la actualización.
             *
             * Ahora sí podemos eliminar la fotografía anterior.
             */

            if (eliminarFotoAnterior &&
                fotoAnterior is not null)
            {
                DeletePhotoFile(fotoAnterior);
            }

            return true;
        }
        catch
        {
            /*
             * Si habíamos creado una nueva foto pero
             * PostgreSQL falla, eliminamos solamente
             * la nueva.
             *
             * La antigua permanece intacta.
             */

            if (nuevaRutaFoto is not null)
            {
                DeletePhotoFile(nuevaRutaFoto);
            }

            throw;
        }
    }


    // =========================================================
    // ELIMINAR
    // =========================================================

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        /*
         * Antes de obtener la entidad hacemos una consulta
         * ligera para comprobar dependencias.
         */

        var info = await _dbContext.Personas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                TieneCasosComoVictima =
                    p.CasosComoVictima.Any(),

                TieneCasosComoAgresor =
                    p.CasosComoPresuntoAgresor.Any(),

                TieneVinculaciones =
                    p.VinculacionesInstitucionales.Any()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (info is null)
        {
            return false;
        }


        /*
         * Una persona vinculada al historial institucional
         * o a un expediente no debe desaparecer físicamente.
         */

        if (info.TieneCasosComoVictima ||
            info.TieneCasosComoAgresor ||
            info.TieneVinculaciones)
        {
            throw new InvalidOperationException(
                "No se puede eliminar la persona porque " +
                "posee información histórica relacionada " +
                "con casos o instituciones.");
        }


        Persona? persona = await _dbContext.Personas
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (persona is null)
        {
            return false;
        }

        string? rutaFoto = persona.RutaFoto;

        _dbContext.Personas.Remove(persona);

        await _dbContext.SaveChangesAsync(
            cancellationToken);


        /*
         * Eliminamos la fotografía solamente después
         * de que PostgreSQL confirme la eliminación.
         */

        if (rutaFoto is not null)
        {
            DeletePhotoFile(rutaFoto);
        }

        return true;
    }


    // =========================================================
    // EXISTENCIA
    // =========================================================

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Personas
            .AsNoTracking()
            .AnyAsync(
                p => p.Id == id,
                cancellationToken);
    }


    // =========================================================
    // VALIDACIONES
    // =========================================================

    /// <summary>
    /// Comprueba que las FK recibidas desde el formulario
    /// realmente existan.
    ///
    /// No debemos confiar únicamente en los valores del
    /// select HTML porque una petición HTTP puede modificarse
    /// manualmente.
    /// </summary>
    private async Task ValidarReferenciasAsync( //-----BORRAR ESTE MÉTODO, NO TIENE SENTIDO, YA QUE NO SE USA EN NINGÚN LADO
        PersonaFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.TipoDocumentoIdentidadId.HasValue)
        {
            bool existeTipoDocumento =
                /*await _dbContext.TipoDocumentosIdentidad
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id ==
                             model.TipoDocumentoIdentidadId.Value,
                        cancellationToken);*/
                await _dbContext.Personas
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id ==
                             model.TipoDocumentoIdentidadId.Value,
                        cancellationToken);

            if (!existeTipoDocumento)
            {
                throw new InvalidOperationException(
                    "El tipo de documento seleccionado no existe.");
            }
        }


        if (model.BarrioId.HasValue)
        {
            bool existeBarrio =
                await _dbContext.Barrios
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == model.BarrioId.Value,
                        cancellationToken);

            if (!existeBarrio)
            {
                throw new InvalidOperationException(
                    "El barrio seleccionado no existe.");
            }
        }
    }


    // =========================================================
    // NORMALIZACIÓN
    // =========================================================

    /// <summary>
    /// Normaliza los datos textuales antes de guardarlos.
    ///
    /// Evita almacenar valores como:
    /// "  Juan  "
    ///
    /// cuando deberían ser:
    /// "Juan"
    /// </summary>
    private static void Normalizar(PersonaFormViewModel model)
    {
        model.Nombres =
            model.Nombres.Trim();

        model.Apellidos =
            model.Apellidos.Trim();

        model.NumeroDocumento =
            NullIfWhiteSpace(model.NumeroDocumento);

        model.Sexo =
            NullIfWhiteSpace(model.Sexo);

        model.Telefono =
            NullIfWhiteSpace(model.Telefono);

        model.Email =
            NullIfWhiteSpace(model.Email);

        model.Direccion =
            NullIfWhiteSpace(model.Direccion);
    }


    private static string? NullIfWhiteSpace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }


    // =========================================================
    // GUARDAR FOTOGRAFÍA
    // =========================================================

    private async Task<string> SavePhotoAsync(IFormFile photo, CancellationToken cancellationToken)
    {
        ValidatePhoto(photo);


        // =====================================================
        // DETERMINAR EXTENSIÓN
        // =====================================================

        /*
         * No utilizamos directamente la extensión enviada
         * en FileName.
         *
         * Obtenemos nosotros la extensión permitida según
         * el ContentType.
         */

        string extension =
            AllowedPhotoTypes[photo.ContentType];


        // =====================================================
        // GENERAR NOMBRE SEGURO
        // =====================================================

        string fileName =
            $"{Guid.NewGuid():N}{extension}";


        // =====================================================
        // CREAR DIRECTORIO
        // =====================================================

        string webRoot =
            _environment.WebRootPath
            ?? Path.Combine(
                _environment.ContentRootPath,
                "wwwroot");

        string directory = Path.Combine(
            webRoot,
            "fotos",
            "personas");

        Directory.CreateDirectory(directory);


        string physicalPath = Path.Combine(
            directory,
            fileName);


        // =====================================================
        // GUARDAR ARCHIVO
        // =====================================================

        await using FileStream stream =
            new FileStream(
                physicalPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

        await photo.CopyToAsync(
            stream,
            cancellationToken);


        /*
         * Guardamos en PostgreSQL una ruta web,
         * no la ruta física del servidor.
         */

        return $"/fotos/personas/{fileName}";
    }


    // =========================================================
    // VALIDACIÓN DE FOTO
    // =========================================================

    private static void ValidatePhoto(IFormFile photo)
    {
        if (photo.Length <= 0)
        {
            throw new InvalidOperationException(
                "La fotografía seleccionada está vacía.");
        }


        if (photo.Length > MaxPhotoSize)
        {
            throw new InvalidOperationException(
                "La fotografía no puede superar los 5 MB.");
        }


        if (string.IsNullOrWhiteSpace(photo.ContentType) ||
            !AllowedPhotoTypes.ContainsKey(
                photo.ContentType))
        {
            throw new InvalidOperationException(
                "Formato de fotografía no permitido. " +
                "Solo se admiten imágenes JPG, PNG o WEBP.");
        }
    }


    // =========================================================
    // ELIMINAR ARCHIVO DE FOTOGRAFÍA
    // =========================================================

    private void DeletePhotoFile(string route)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                return;
            }

            string webRoot =
                _environment.WebRootPath
                ?? Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");


            /*
             * Convertimos:
             *
             * /fotos/personas/abc.jpg
             *
             * en:
             *
             * wwwroot/fotos/personas/abc.jpg
             */

            string relativePath = route
                .TrimStart('/')
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);


            string physicalPath = Path.Combine(
                webRoot,
                relativePath);


            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
        catch (Exception ex)
        {
            /*
             * Un fallo al borrar físicamente una foto antigua
             * no debe revertir una operación de BD que ya fue
             * confirmada.
             *
             * Lo dejamos registrado para diagnóstico.
             */

            _logger.LogWarning(
                ex,
                "No se pudo eliminar la fotografía física {RutaFoto}.",
                route);
        }
    }
}