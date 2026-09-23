using e_violenciagen.Models;
using e_violenciagen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace e_violenciagen.Controllers;
public class CuentaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;


    public CuentaController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    // =========================================================
    // LOGIN - GET
    // =========================================================

    /*
     * AllowAnonymous es importante porque el usuario
     * todavía no está autenticado.
     */
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        /*
         * Si el usuario ya está autenticado,
         * no tiene sentido volver a mostrarle el login.
         */
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Home");
        }


        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }


    // =========================================================
    // LOGIN - POST
    // =========================================================

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        /*
         * Buscamos primero el usuario.
         *
         * Esto nos permite aplicar nuestra regla
         * adicional "Activo" antes del login.
         */
        ApplicationUser? usuario =
            await _userManager.FindByNameAsync(
                model.NombreUsuario);


        /*
         * IMPORTANTE:
         *
         * Utilizamos un mensaje genérico.
         *
         * No queremos revelar si:
         *
         * - el usuario no existe;
         * - la contraseña es incorrecta.
         *
         * Esto reduce información útil
         * para intentos de enumeración de cuentas.
         */
        if (usuario is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Usuario o contraseña incorrectos.");

            return View(model);
        }


        // =====================================================
        // CUENTA DESACTIVADA ADMINISTRATIVAMENTE
        // =====================================================

        if (!usuario.Activo)
        {
            ModelState.AddModelError(
                string.Empty,
                "Esta cuenta no está disponible.");

            return View(model);
        }


        // =====================================================
        // AUTENTICACIÓN CON IDENTITY
        // =====================================================

        SignInResult resultado =
            await _signInManager.PasswordSignInAsync(
                usuario,
                model.Password,
                model.Recordarme,

                /*
                 * Si la contraseña es incorrecta,
                 * Identity incrementará AccessFailedCount.
                 *
                 * Esto permitirá posteriormente
                 * bloquear cuentas automáticamente.
                 */
                lockoutOnFailure: true);


        if (resultado.Succeeded)
        {
            /*
             * Registramos el último acceso correcto.
             */
            usuario.UltimoAcceso = DateTime.UtcNow;

            IdentityResult updateResult =
                await _userManager.UpdateAsync(usuario);

            /*
             * El login ya fue correcto.
             *
             * Un error actualizando UltimoAcceso
             * no debería invalidar la autenticación,
             * pero por ahora dejamos que Identity
             * gestione normalmente la actualización.
             */
            if (!updateResult.Succeeded)
            {
                // Más adelante podemos registrar
                // este error mediante ILogger.
            }


            // =================================================
            // REDIRECCIÓN SEGURA
            // =================================================

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
                && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }


            return RedirectToAction(
                "Index",
                "Home");
        }


        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "La cuenta está temporalmente bloqueada.");

            return View(model);
        }


        if (resultado.IsNotAllowed)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se permite iniciar sesión con esta cuenta.");

            return View(model);
        }


        ModelState.AddModelError(
            string.Empty,
            "Usuario o contraseña incorrectos.");

        return View(model);
    }


    // =========================================================
    // LOGOUT
    // =========================================================

    /*
     * Logout sí exige usuario autenticado.
     */
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            nameof(Login));
    }
}