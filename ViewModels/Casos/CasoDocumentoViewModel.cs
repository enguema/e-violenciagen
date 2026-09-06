namespace e_violenciagen.ViewModels.Casos;
public class CasoDocumentoViewModel
{
    public Guid Id { get; set; }

    public string NombreOriginal { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;

    public DateOnly? FechaDocumento { get; set; }

    public DateTime FechaIncorporacion { get; set; }

    public string? Titulo { get; set; }

    public string? TipoMime { get; set; }

    public long TamanoBytes { get; set; }

    public bool EsConfidencial { get; set; }

    public Guid? ActuacionId { get; set; }

    public string? ActuacionTitulo { get; set; }
}