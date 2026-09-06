using e_violenciagen.ViewModels;
using e_violenciagen.ViewModels.Casos;

namespace e_violenciagen.Aplication.Casos;
/// <summary>
/// Define las operaciones disponibles para consultar
/// y gestionar expedientes de violencia.
/// </summary>
public interface ICasoService
{
    /// <summary>
    /// Obtiene el listado resumido de expedientes.
    /// </summary>
    Task<IReadOnlyList<CasoIndexItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Obtiene toda la información necesaria para mostrar
    /// el expediente electrónico.
    ///
    /// Devuelve null cuando el caso no existe.
    /// </summary>
    Task<CasoDetailsViewModel?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);


    /// <summary>
    /// Obtiene únicamente las actuaciones de un caso.
    ///
    /// Será utilizado posteriormente mediante Fetch
    /// para actualizar parcialmente la interfaz.
    /// </summary>
    Task<IReadOnlyList<CasoActuacionViewModel>> GetActuacionesAsync(Guid casoId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Obtiene únicamente los documentos del expediente.
    /// </summary>
    Task<IReadOnlyList<CasoDocumentoViewModel>> GetDocumentosAsync(Guid casoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Construye el formulario inicial con sus catálogos.
    /// </summary>
    Task<CasoCreateViewModel> GetCreateFormAsync(
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Registra atómicamente un nuevo expediente.
    /// </summary>
    Task<CasoCreateResultViewModel> CreateAsync(
        CasoCreateViewModel model,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Distritos pertenecientes a una provincia.
    /// </summary>
    Task<IReadOnlyList<OpcionCatalogoViewModel>> GetDistritosAsync(
        Guid provinciaId,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Barrios pertenecientes a un distrito.
    /// </summary>
    Task<IReadOnlyList<OpcionCatalogoViewModel>> GetBarriosAsync(
        Guid distritoId,
        CancellationToken cancellationToken = default);
}