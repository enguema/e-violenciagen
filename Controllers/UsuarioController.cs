using e_violenciagen.Aplication;
using e_violenciagen.Models;
using e_violenciagen.ViewModels;
using e_violenciagen.ViewModels.Seguridad.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_violenciagen.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;


    public UsuarioController(
        IUsuarioService usuarioService,
        ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosVer)]
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var usuarios =
            await _usuarioService.GetAllAsync(
                cancellationToken);

        return View(usuarios);
    }

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        UsuarioCreateViewModel model =
            await _usuarioService.PrepararCreateAsync(
                cancellationToken);

        return View(model);
    }

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            /*
             * Si el formulario falla, debemos volver
             * a cargar Instituciones y Roles.
             */
            UsuarioCreateViewModel datos =
                await _usuarioService.PrepararCreateAsync(
                    cancellationToken);

            model.Instituciones =
                datos.Instituciones;

            model.Roles =
                datos.Roles;

            return View(model);
        }


        try
        {
            await _usuarioService.CrearAsync(
                model,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Usuario creado correctamente.";

            return RedirectToAction(
                nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            /*
             * Mostramos el problema en la misma vista,
             * manteniendo el patrón que ya estás usando
             * en el proyecto.
             */
            ModelState.AddModelError(
                string.Empty,
                ex.Message);


            UsuarioCreateViewModel datos =
                await _usuarioService.PrepararCreateAsync(
                    cancellationToken);

            model.Instituciones =
                datos.Instituciones;

            model.Roles =
                datos.Roles;

            return View(model);
        }
    }

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
    [HttpGet]
    public async Task<IActionResult> UnidadesPorInstitucion(Guid institucionId, CancellationToken cancellationToken)
    {
        var unidades =
            await _usuarioService
                .GetUnidadesPorInstitucionAsync(
                    institucionId,
                    cancellationToken);

        return Json(unidades);
    }

    [Authorize(
    Policy =
        PermisosSistema.Seguridad.UsuariosCambiarEstado)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _usuarioService.CambiarEstadoAsync(
                id,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Estado del usuario actualizado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;
        }


        return RedirectToAction(
            nameof(Index));
    }
}