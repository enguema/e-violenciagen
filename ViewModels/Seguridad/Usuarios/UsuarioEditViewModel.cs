using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace e_violenciagen.ViewModels;

public class UsuarioEditViewModel
{
    public Guid Id { get; set; }


    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;


    [Required]
    [MaxLength(150)]
    public string Apellidos { get; set; } = string.Empty;


    [Required]
    [Display(Name = "Nombre de usuario")]
    public string NombreUsuario { get; set; } = string.Empty;


    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }


    [Required]
    [Display(Name = "Institución")]
    public Guid? InstitucionId { get; set; }


    [Display(Name = "Unidad organizativa")]
    public Guid? UnidadOrganizativaId { get; set; }


    public List<Guid> RolesSeleccionados { get; set; } = [];


    public IEnumerable<SelectListItem> Instituciones { get; set; }
        = [];

    public IEnumerable<SelectListItem> UnidadesOrganizativas { get; set; }
        = [];

    public IEnumerable<SelectListItem> Roles { get; set; }
        = [];
}