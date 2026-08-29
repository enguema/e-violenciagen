using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using e_violenciagen.Data;

namespace e_violenciagen.Controllers;

/// <summary>
/// Controller temporal destinado a verificar
/// la infraestructura básica de SIGEVIG.
///
/// Más adelante podremos eliminarlo o restringir
/// su acceso exclusivamente a administradores.
/// </summary>
public class InfraestructuraController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public InfraestructuraController(
        AppDbContext dbContext,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }


    /// <summary>
    /// Comprueba si la aplicación puede comunicarse
    /// correctamente con PostgreSQL.
    ///
    /// GET /sistema/estado
    /// </summary>
    [HttpGet("/sistema/estado")]
    public async Task<IActionResult> Estado(CancellationToken cancellationToken)
    {
        try
        {
            bool databaseConnected =
                await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (!databaseConnected)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        success = false,
                        application = "SIGEVIG",
                        database = "No disponible"
                    });
            }

            return Ok(new
            {
                success = true,
                application = "SIGEVIG",
                environment = _environment.EnvironmentName,
                database = "PostgreSQL conectado"
            });
        }
        catch
        {
            /*
             * No devolvemos la excepción completa al navegador
             * porque podría revelar información interna de la
             * infraestructura.
             */
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new
                {
                    success = false,
                    application = "SIGEVIG",
                    database = "Error de conexión"
                });
        }
    }

    
}