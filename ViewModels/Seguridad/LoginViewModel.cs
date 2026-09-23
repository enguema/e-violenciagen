using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels;
/// <summary>
/// Datos requeridos para iniciar sesión.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Debe indicar el nombre de usuario.")]
    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;


    [Required(ErrorMessage = "Debe indicar la contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;


    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }


    /*
     * Permite regresar a la página solicitada
     * después del inicio de sesión.
     */
    public string? ReturnUrl { get; set; }
}