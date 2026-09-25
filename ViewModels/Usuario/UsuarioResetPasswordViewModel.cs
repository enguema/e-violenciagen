using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels;
public class UsuarioResetPasswordViewModel
{
    public Guid UsuarioId { get; set; }


    public string NombreUsuario { get; set; }
        = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; }
        = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    [Compare(
        nameof(NuevaPassword),
        ErrorMessage =
            "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; }
        = string.Empty;
}