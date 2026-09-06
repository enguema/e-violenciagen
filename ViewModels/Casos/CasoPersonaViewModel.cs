namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Información resumida de una persona dentro del expediente.
///
/// Se reutilizará tanto para víctimas como para
/// presuntos agresores.
/// </summary>
public class CasoPersonaViewModel
{
    public Guid PersonaId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Telefono { get; set; }

    public bool EsPrincipal { get; set; }

    public bool RequiereProteccion { get; set; }

    public string? RelacionConVictima { get; set; }

    public string? Observaciones { get; set; }
}