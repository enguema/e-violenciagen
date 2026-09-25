using System.Security.Claims;
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


    public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosVer)]
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var usuarios =
            await _usuarioService.GetAllAsync(
                cancellationToken);

        return View(usuarios);
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        UsuarioCreateViewModel model =
            await _usuarioService.PrepararCreateAsync(
                cancellationToken);

        return View(model);
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
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

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosCrear)]
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

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosCambiarEstado)]
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

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosEditar)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        UsuarioEditViewModel? model =
            await _usuarioService.PrepararEditAsync(
                id,
                cancellationToken);

        if (model is null)
        {
            return NotFound();
        }


        return View(model);
    }

   // [Authorize(Policy = PermisosSistema.Seguridad.UsuariosEditar)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    UsuarioEditViewModel model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            UsuarioEditViewModel? datos =
                await _usuarioService.PrepararEditAsync(
                    model.Id,
                    cancellationToken);

            if (datos is null)
            {
                return NotFound();
            }

            model.Instituciones =
                datos.Instituciones;

            model.UnidadesOrganizativas =
                datos.UnidadesOrganizativas;

            model.Roles =
                datos.Roles;

            return View(model);
        }


        string? idActual = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
            idActual,
            out Guid usuarioActualId))
        {
            return Unauthorized();
        }


        try
        {
            await _usuarioService.ActualizarAsync(
                model,
                usuarioActualId,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Usuario actualizado correctamente.";

            return RedirectToAction(
                nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);


            UsuarioEditViewModel? datos =
                await _usuarioService.PrepararEditAsync(
                    model.Id,
                    cancellationToken);

            if (datos is null)
            {
                return NotFound();
            }


            model.Instituciones =
                datos.Instituciones;

            model.UnidadesOrganizativas =
                datos.UnidadesOrganizativas;

            model.Roles =
                datos.Roles;

            return View(model);
        }
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosResetPassword)]
    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var model =
            await _usuarioService
                .PrepararResetPasswordAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Policy = PermisosSistema.Seguridad.UsuariosResetPassword)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(UsuarioResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _usuarioService
                .RestablecerPasswordAsync(model);

            TempData["SuccessMessage"] =
                "Contraseña restablecida correctamente.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.UsuarioId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model);
        }
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosDesbloquear)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desbloquear(Guid id)
    {
        try
        {
            await _usuarioService.DesbloquearAsync(id);

            TempData["SuccessMessage"] =
                "Usuario desbloqueado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosVer)]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        UsuarioDetailViewModel? model =
            await _usuarioService.GetByIdAsync(
                id,
                cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    //[Authorize(Policy = PermisosSistema.Seguridad.UsuariosVer)]
    [HttpPost]
    public async Task<IActionResult> DataTable(CancellationToken cancellationToken)
    {
        /*
         * Parámetros estándar enviados por DataTables.
         */
        int draw = int.TryParse(
                Request.Form["draw"],
                out int drawValue)
                ? drawValue
                : 0;

        int start = int.TryParse(
                Request.Form["start"],
                out int startValue)
                ? startValue
                : 0;

        int length = int.TryParse(
                Request.Form["length"],
                out int lengthValue)
                ? lengthValue
                : 10;


        string? search = Request.Form["search[value]"]
                .FirstOrDefault();


        var resultado = await _usuarioService.GetDataTableAsync(
                draw,
                start,
                length,
                search,
                cancellationToken);


        return Json(resultado);
    }

}