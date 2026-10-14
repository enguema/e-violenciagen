using e_violenciagen.Data;
using e_violenciagen.ViewModels;
using Microsoft.EntityFrameworkCore;

public class InstitucionService : IInstitucionService
{
    private readonly AppDbContext _dbContext;

    public InstitucionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<UnidadOrganizativaOpcionViewModel>>
    ObtenerUnidadesPorInstitucionAsync(Guid institucionId, CancellationToken cancellationToken = default)
    {
        /*
         * No devolvemos entidades EF.
         *
         * Solo enviamos al cliente los datos mínimos
         * necesarios para construir el desplegable.
         */
        return await _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .Where(u => u.InstitucionId == institucionId)
            .OrderBy(u => u.Nombre)
            .Select(u => new UnidadOrganizativaOpcionViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre
            })
            .ToListAsync(cancellationToken);
    }
}