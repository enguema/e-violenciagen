namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Representa una actuación en la línea temporal
/// del expediente.
/// </summary>
public class CasoActuacionViewModel
{
    public Guid Id { get; set; }

    public DateTime FechaActuacion { get; set; }

    public string TipoActuacion { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Resultado { get; set; }

    public string Institucion { get; set; } = string.Empty;

    public string? UnidadOrganizativa { get; set; }

    public int NumeroParticipantes { get; set; }

    public int NumeroDocumentos { get; set; }
}