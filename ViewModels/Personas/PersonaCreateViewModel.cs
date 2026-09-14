using e_violenciagen.ViewModels.Personas;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace e_violenciagen.ViewModels.Personas;
/// <summary>
/// ViewModel específico de la pantalla Create.
///
/// Persona contiene los datos introducidos por el usuario.
///
/// TiposDocumento y Barrios contienen únicamente las opciones
/// necesarias para construir los select de la vista.
/// </summary>
public class PersonaCreateViewModel
{
    public PersonaFormViewModel Persona { get; set; }
        = new();

    public IReadOnlyList<SelectListItem> TiposDocumento { get; set; }
        = Array.Empty<SelectListItem>();

    public IReadOnlyList<SelectListItem> Barrios { get; set; }
        = Array.Empty<SelectListItem>();
}