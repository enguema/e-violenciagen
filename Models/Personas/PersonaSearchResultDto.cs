namespace e_violenciagen.Models.Personas;
/// <summary>
/// DTO ligero utilizado por el buscador reutilizable
/// de Personas.
///
/// Debe contener únicamente la información necesaria
/// para que el usuario pueda identificar correctamente
/// a la persona encontrada.
/// </summary>
public class PersonaSearchResultDto
{
    public Guid Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? RutaFoto { get; set; }
    public bool Activo { get; set; }
}