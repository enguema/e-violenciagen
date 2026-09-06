namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Representa todo lo necesario para mostrar
/// el expediente electrónico de un caso.
/// </summary>
public class CasoDetailsViewModel
{
    public Guid Id { get; set; }

    public string CodigoCaso { get; set; } = string.Empty;


    // =====================================================
    // ESTADO Y FECHAS
    // =====================================================

    public string Estado { get; set; } = string.Empty;

    public string CodigoEstado { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public DateOnly? FechaHecho { get; set; }


    // =====================================================
    // INFORMACIÓN GENERAL
    // =====================================================

    public string? Resumen { get; set; }

    public string? RelatoInicial { get; set; }

    public string? LugarDescripcion { get; set; }


    // =====================================================
    // TERRITORIO
    // =====================================================

    public string? Provincia { get; set; }

    public string? Distrito { get; set; }

    public string? Barrio { get; set; }


    // =====================================================
    // CLASIFICACIÓN
    // =====================================================

    public List<string> TiposViolencia { get; set; } = [];


    // =====================================================
    // PERSONAS
    // =====================================================

    public List<CasoPersonaViewModel> Victimas { get; set; } = [];

    public List<CasoPersonaViewModel> PresuntosAgresores { get; set; } = [];


    // =====================================================
    // ACTUACIONES
    // =====================================================

    public List<CasoActuacionViewModel> Actuaciones { get; set; } = [];


    // =====================================================
    // DOCUMENTOS
    // =====================================================

    public List<CasoDocumentoViewModel> Documentos { get; set; } = [];
}