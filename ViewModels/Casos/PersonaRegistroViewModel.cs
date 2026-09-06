namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Datos básicos necesarios para identificar o registrar
/// una persona mientras se crea un expediente.
///
/// Las propiedades no llevan [Required] aquí porque este mismo
/// modelo se utiliza para el presunto agresor, que es opcional.
///
/// Las reglas obligatorias se validarán desde CasoCreateViewModel.
/// </summary>
public class PersonaRegistroViewModel
{
    public Guid? TipoDocumentoIdentidadId { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }

    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}