using e_violenciagen.Aplication.Casos;
using e_violenciagen.Models;
using e_violenciagen.ViewModels.Casos;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize(Policy = PermisosSistema.Casos.Ver)]
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var casos = await _casoService.GetAllAsync(cancellationToken);

        return View(casos);
    }

    // =========================================================
    // CREAR CASO - FORMULARIO
    // =========================================================

    //[Authorize(Policy = PermisosSistema.Casos.Crear)]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model =
            await _casoService.GetCreateFormAsync(
                cancellationToken);

        return View(model);
    }


    // =========================================================
    // CREAR CASO - GUARDAR
    // =========================================================

    ///[Authorize(Policy = PermisosSistema.Casos.Crear)]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarVictima(
    AgregarVictimaViewModel model,
    CancellationToken cancellationToken)
    {
        /*
         * Necesitamos siempre el CasoId para poder regresar
         * al expediente aunque haya un error.
         */
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Debe seleccionar una persona.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.CasoId });
        }

        try
        {
            await _casoService.AgregarVictimaAsync(
                model.CasoId,
                model.PersonaId!.Value,
                cancellationToken);

            TempData["Success"] =
                "La víctima fue agregada correctamente al expediente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al agregar una víctima al caso {CasoId}.",
                model.CasoId);

            TempData["Error"] =
                "Se produjo un error al agregar la víctima.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = model.CasoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarPresuntoAgresor(
    AgregarPresuntoAgresorViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Debe seleccionar una persona.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.CasoId });
        }

        try
        {
            await _casoService.AgregarPresuntoAgresorAsync(
                model.CasoId,
                model.PersonaId!.Value,
                cancellationToken);

            TempData["Success"] =
                "El presunto agresor fue agregado correctamente al expediente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al agregar un presunto agresor al caso {CasoId}.",
                model.CasoId);

            TempData["Error"] =
                "Se produjo un error al agregar el presunto agresor.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = model.CasoId });
    }

    // =========================================================
    // EXPEDIENTE
    // =========================================================
    //[Authorize(Policy = PermisosSistema.Casos.Ver)]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Casos.Editar")]
    public async Task<IActionResult> RegistrarActuacion(
    RegistrarActuacionViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Revise los datos introducidos para la actuación.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.CasoId });
        }

        try
        {
            await _casoService.RegistrarActuacionAsync(
                model,
                cancellationToken);

            TempData["Success"] =
                "La actuación institucional fue registrada correctamente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al registrar una actuación para el caso {CasoId}.",
                model.CasoId);

            TempData["Error"] =
                "Se produjo un error al registrar la actuación.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = model.CasoId });
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

    // Vincular y desvincular personas a un caso
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DesvincularVictima(
    DesvincularVictimaViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "No se pudo identificar correctamente la víctima.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.CasoId });
        }

        try
        {
            await _casoService.DesvincularVictimaAsync(
                model.CasoId,
                model.PersonaId,
                cancellationToken);

            TempData["Success"] =
                "La víctima fue desvinculada correctamente del expediente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al desvincular la víctima {PersonaId} del caso {CasoId}.",
                model.PersonaId,
                model.CasoId);

            TempData["Error"] =
                "Se produjo un error al desvincular la víctima.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = model.CasoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DesvincularPresuntoAgresor(
    DesvincularPresuntoAgresorViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "No se pudo identificar correctamente al presunto agresor.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.CasoId });
        }

        try
        {
            await _casoService.DesvincularPresuntoAgresorAsync(
                model.CasoId,
                model.PersonaId,
                cancellationToken);

            TempData["Success"] =
                "El presunto agresor fue desvinculado correctamente del expediente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al desvincular el presunto agresor {PersonaId} del caso {CasoId}.",
                model.PersonaId,
                model.CasoId);

            TempData["Error"] =
                "Se produjo un error al desvincular al presunto agresor.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = model.CasoId });
    }

}