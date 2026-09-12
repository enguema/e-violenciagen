using e_violenciagen.Aplication.Personas;
using Microsoft.AspNetCore.Mvc;
using e_violenciagen.ViewModels.Common;
using e_violenciagen.ViewModels.Personas;

namespace e_violenciagen.Controllers;

/// <summary>
/// Controlador MVC responsable de la gestión de Personas.
///
/// Su responsabilidad se limita a:
///
/// - recibir peticiones HTTP;
/// - validar los datos recibidos;
/// - delegar la lógica en IPersonaService;
/// - devolver vistas o respuestas JSON.
///
/// No contiene lógica de EF Core ni manipulación física
/// de fotografías.
/// </summary>
public class PersonaController : Controller
{
    private readonly IPersonaService _personaService;
    private readonly ILogger<PersonaController> _logger;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public PersonaController(
        IPersonaService personaService,
        ILogger<PersonaController> logger)
    {
        _personaService = personaService;
        _logger = logger;
    }


    // =========================================================
    // INDEX
    // =========================================================

    /// <summary>
    /// Muestra la pantalla principal de Personas.
    ///
    /// IMPORTANTE:
    /// Esta acción NO carga la lista de personas.
    ///
    /// El DataTable solicitará los datos posteriormente
    /// mediante AJAX a la acción DataTable.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }


    // =========================================================
    // DATATABLE SERVER SIDE
    // =========================================================

    /// <summary>
    /// Atiende las solicitudes AJAX generadas por DataTables
    /// cuando serverSide = true.
    ///
    /// Toda la búsqueda, ordenación y paginación ocurre
    /// posteriormente en PersonaService/PostgreSQL.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DataTable(
        DataTableRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _personaService
                .GetDataTableAsync(
                    request,
                    cancellationToken);

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener el listado de personas.");

            /*
             * DataTables espera una respuesta JSON.
             *
             * No devolvemos detalles internos de la excepción
             * al navegador.
             */
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message =
                        "No se pudo obtener el listado de personas."
                });
        }
    }


    // =========================================================
    // DETAILS
    // =========================================================

    /// <summary>
    /// Muestra la ficha detallada de una persona.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        var persona = await _personaService.GetByIdAsync(
            id,
            cancellationToken);

        if (persona is null)
        {
            return NotFound();
        }

        return View(persona);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    /// <summary>
    /// Muestra el formulario para registrar una nueva persona.
    ///
    /// Los catálogos necesarios para los selects
    /// los incorporaremos cuando construyamos la vista Create.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        var model = new PersonaFormViewModel();

        return View(model);
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    /// <summary>
    /// Registra una nueva persona.
    ///
    /// La operación está preparada para ser invocada mediante
    /// fetch/AJAX usando FormData, necesario por la fotografía.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PersonaFormViewModel model,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // VALIDACIÓN DEL VIEWMODEL
        // =====================================================

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,

                message =
                    "Revise los datos introducidos.",

                errors = GetModelStateErrors()
            });
        }


        try
        {
            Guid personaId = await _personaService.CreateAsync(
                model,
                cancellationToken);


            return Json(new
            {
                success = true,

                message =
                    "La persona se ha registrado correctamente.",

                id = personaId
            });
        }
        catch (InvalidOperationException ex)
        {
            /*
             * InvalidOperationException representa aquí
             * una regla funcional conocida:
             *
             * - catálogo inexistente;
             * - fotografía inválida;
             * - otra validación de negocio.
             */

            return UnprocessableEntity(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al registrar una nueva persona.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,

                    message =
                        "Ocurrió un error al registrar la persona."
                });
        }
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    /// <summary>
    /// Recupera los datos de la persona en formato
    /// PersonaFormViewModel para mostrarlos en Edit.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        PersonaFormViewModel? model =
            await _personaService.GetForEditAsync(
                id,
                cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }


    // =========================================================
    // EDIT - POST
    // =========================================================

    /// <summary>
    /// Actualiza una persona existente.
    ///
    /// Puede:
    /// - mantener la fotografía actual;
    /// - sustituirla;
    /// - eliminarla.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        PersonaFormViewModel model,
        CancellationToken cancellationToken)
    {
        /*
         * El identificador de la URL es la fuente fiable.
         *
         * No confiamos en un Id enviado desde un input hidden,
         * ya que el navegador puede manipularlo.
         */
        model.Id = id;


        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,

                message =
                    "Revise los datos introducidos.",

                errors = GetModelStateErrors()
            });
        }


        try
        {
            bool updated = await _personaService.UpdateAsync(
                id,
                model,
                cancellationToken);


            if (!updated)
            {
                return NotFound(new
                {
                    success = false,

                    message =
                        "La persona indicada no existe."
                });
            }


            return Json(new
            {
                success = true,

                message =
                    "Los datos de la persona se han actualizado correctamente.",

                id
            });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al actualizar la persona {PersonaId}.",
                id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,

                    message =
                        "Ocurrió un error al actualizar la persona."
                });
        }
    }


    // =========================================================
    // DELETE
    // =========================================================

    /// <summary>
    /// Elimina una persona cuando las reglas del dominio
    /// permiten la operación.
    ///
    /// Se invocará mediante AJAX para evitar una recarga
    /// completa de la página.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _personaService.DeleteAsync(
                id,
                cancellationToken);


            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,

                    message =
                        "La persona indicada no existe."
                });
            }


            return Json(new
            {
                success = true,

                message =
                    "La persona se ha eliminado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            /*
             * Por ejemplo:
             *
             * La persona participa en uno o más casos
             * y por tanto no puede eliminarse.
             */

            return UnprocessableEntity(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al eliminar la persona {PersonaId}.",
                id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,

                    message =
                        "Ocurrió un error al eliminar la persona."
                });
        }
    }


    // =========================================================
    // MÉTODOS PRIVADOS
    // =========================================================

    /// <summary>
    /// Convierte los errores de ModelState en una estructura
    /// sencilla que JavaScript pueda interpretar.
    ///
    /// Ejemplo:
    ///
    /// {
    ///     "Nombres": [
    ///         "Los nombres son obligatorios."
    ///     ],
    ///     "Email": [
    ///         "Introduzca un correo electrónico válido."
    ///     ]
    /// }
    /// </summary>
    private Dictionary<string, string[]> GetModelStateErrors()
    {
        return ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(error =>
                        string.IsNullOrWhiteSpace(
                            error.ErrorMessage)
                            ? "Valor no válido."
                            : error.ErrorMessage)
                    .ToArray());
    }
}