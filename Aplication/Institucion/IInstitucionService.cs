using e_violenciagen.ViewModels;

public interface IInstitucionService
{
    Task<List<UnidadOrganizativaOpcionViewModel>>
    ObtenerUnidadesPorInstitucionAsync(
        Guid institucionId,
        CancellationToken cancellationToken = default);
}