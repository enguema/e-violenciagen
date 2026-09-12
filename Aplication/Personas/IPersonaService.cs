using e_violenciagen.Dtos.Common;
using e_violenciagen.Dtos.Personas;
using e_violenciagen.ViewModels.Common;
using e_violenciagen.ViewModels.Personas;

namespace e_violenciagen.Aplication.Personas;

/// <summary>
/// Define las operaciones disponibles para la gestión de personas.
///
/// Esta interfaz no conoce detalles de EF Core, PostgreSQL,
/// almacenamiento físico de fotografías ni del controlador.
///
/// Esos detalles corresponden a la implementación PersonaService.
/// </summary>
public interface IPersonaService
{
    // =========================================================
    // CONSULTA PARA DATATABLE
    // =========================================================

    /// <summary>
    /// Obtiene una página de personas adaptada al funcionamiento
    /// serverSide de DataTables.
    ///
    /// La implementación deberá realizar búsqueda, ordenación,
    /// conteo y paginación directamente en PostgreSQL.
    ///
    /// No debe cargar todas las personas en memoria.
    /// </summary>
    Task<DataTableResponse<PersonaListItemDto>> GetDataTableAsync(DataTableRequest request, CancellationToken cancellationToken = default);


    // =========================================================
    // DETALLE
    // =========================================================

    /// <summary>
    /// Obtiene toda la información necesaria para mostrar
    /// la ficha detallada de una persona.
    ///
    /// Devuelve null cuando la persona no existe.
    /// </summary>
    Task<PersonaDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);


    // =========================================================
    // PREPARACIÓN DEL FORMULARIO DE EDICIÓN
    // =========================================================

    /// <summary>
    /// Obtiene los datos actuales de una persona en el formato
    /// utilizado por el formulario de edición.
    ///
    /// Esto evita exponer directamente la entidad Persona
    /// al controlador y a la vista.
    /// </summary>
    Task<PersonaFormViewModel?> GetForEditAsync(Guid id, CancellationToken cancellationToken = default);


    // =========================================================
    // CREACIÓN
    // =========================================================

    /// <summary>
    /// Registra una nueva persona.
    ///
    /// La implementación será responsable de:
    ///
    /// - validar reglas de negocio;
    /// - guardar la fotografía cuando exista;
    /// - almacenar la ruta de la fotografía;
    /// - persistir la persona en PostgreSQL.
    ///
    /// Devuelve el identificador de la nueva persona.
    /// </summary>
    Task<Guid> CreateAsync(PersonaFormViewModel model, CancellationToken cancellationToken = default);


    // =========================================================
    // EDICIÓN
    // =========================================================

    /// <summary>
    /// Actualiza los datos de una persona existente.
    ///
    /// También deberá gestionar correctamente:
    /// - mantenimiento de la fotografía actual;
    /// - sustitución de fotografía;
    /// - eliminación explícita de fotografía.
    /// </summary>
    Task<bool> UpdateAsync( Guid id, PersonaFormViewModel model, CancellationToken cancellationToken = default);


    // =========================================================
    // ELIMINACIÓN
    // =========================================================

    /// <summary>
    /// Elimina una persona cuando las reglas del dominio
    /// permitan realizar la operación.
    ///
    /// En fases posteriores comprobaremos si una persona
    /// vinculada a casos puede realmente eliminarse físicamente
    /// o si debe aplicarse otra estrategia.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);


    // =========================================================
    // COMPROBACIÓN DE EXISTENCIA
    // =========================================================

    /// <summary>
    /// Comprueba si existe una persona con el identificador dado.
    ///
    /// Puede ser útil para validaciones ligeras sin necesidad
    /// de cargar todos los datos de la persona.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}