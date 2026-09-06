using e_violenciagen.Aplication.Casos;
using e_violenciagen.ViewModels.Casos;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Gestiona las páginas relacionadas con los expedientes.
///
/// El Controller coordina la petición HTTP,
/// pero no contiene consultas directas a EF Core.
/// </summary>
public class CasoController : Controller
{
    private readonly ICasoService _casoService;
    private readonly ILogger<CasoController> _logger;

    public CasoController(
        ICasoService casoService,
        ILogger<CasoController> logger)
    {
        _casoService = casoService;
        _logger = logger;
    }


    // =========================================================
    // LISTADO
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var casos = await _casoService.GetAllAsync(cancellationToken);

        return View(casos);
    }

    // =========================================================
    // CREAR CASO - FORMULARIO
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var model =
            await _casoService.GetCreateFormAsync(
                cancellationToken);

        return View(model);
    }


    // =========================================================
    // CREAR CASO - GUARDAR
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CasoCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            /*
             * Como el formulario se envía mediante Fetch,
             * devolvemos una respuesta JSON coherente.
             */
            return BadRequest(new
            {
                success = false,
                message = "Revise los datos introducidos.",

                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray())
            });
        }


        try
        {
            CasoCreateResultViewModel resultado =
                await _casoService.CreateAsync(
                    model,
                    cancellationToken);

            return Ok(new
            {
                success = true,

                message =
                    $"El expediente {resultado.CodigoCaso} " +
                    "se registró correctamente.",

                id = resultado.Id,

                redirectUrl =
                    Url.Action(
                        nameof(Details),
                        "Caso",
                        new { id = resultado.Id })
            });
        }
        catch (InvalidOperationException ex)
        {
            /*
             * 422 indica que la petición tiene formato válido,
             * pero incumple una regla de negocio.
             */
            return UnprocessableEntity(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    // =========================================================
    // EXPEDIENTE
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var caso = await _casoService.GetDetailsAsync(
                id,
                cancellationToken);

        if (caso is null)
        {
            return NotFound();
        }

        return View(caso);
    }


    // =========================================================
    // PARTIAL: ACTUACIONES
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Actuaciones(Guid casoId, CancellationToken cancellationToken)
    {
        var actuaciones = await _casoService.GetActuacionesAsync(
                casoId,
                cancellationToken);

        return PartialView(
            "_Actuaciones",
            actuaciones);
    }


    // =========================================================
    // PARTIAL: DOCUMENTOS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Documentos(Guid casoId, CancellationToken cancellationToken)
    {
        var documentos =
            await _casoService.GetDocumentosAsync(
                casoId,
                cancellationToken);

        return PartialView(
            "_Documentos",
            documentos);
    }

    // =========================================================
    // TERRITORIO
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Distritos(Guid provinciaId, CancellationToken cancellationToken)
    {
        var distritos =
            await _casoService.GetDistritosAsync(
                provinciaId,
                cancellationToken);

        return Json(distritos);
    }


    [HttpGet]
    public async Task<IActionResult> Barrios(Guid distritoId, CancellationToken cancellationToken)
    {
        var barrios =
            await _casoService.GetBarriosAsync(
                distritoId,
                cancellationToken);

        return Json(barrios);
    }
}