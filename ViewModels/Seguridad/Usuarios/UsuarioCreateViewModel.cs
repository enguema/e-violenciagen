using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace e_violenciagen.ViewModels.Seguridad.Usuarios;
public class UsuarioCreateViewModel
{
    [Required]
    [Display(Name = "Nombre")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Apellidos")]
    [MaxLength(150)]
    public string Apellidos { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Nombre de usuario")]
    public string NombreUsuario { get; set; } = string.Empty;


    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }


    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña inicial")]
    public string Password { get; set; } = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Institución")]
    public Guid? InstitucionId { get; set; }


    [Display(Name = "Unidad organizativa")]
    public Guid? UnidadOrganizativaId { get; set; }


    /*
     * Permitimos asignar varios roles.
     */
    public List<Guid> RolesSeleccionados { get; set; } = [];


    // =====================================================
    // DATOS PARA LOS SELECTS
    // =====================================================

    public IEnumerable<SelectListItem> Instituciones { get; set; }
        = [];

    public IEnumerable<SelectListItem> UnidadesOrganizativas { get; set; }
        = [];

    public IEnumerable<SelectListItem> Roles { get; set; }
        = [];
}