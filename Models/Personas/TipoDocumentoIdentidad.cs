namespace e_violenciagen.Models;
/// <summary>
/// Catálogo que identifica los diferentes documentos
/// utilizados para identificar a una persona.
///
/// No debe confundirse con TipoDocumentoExpediente,
/// que clasifica archivos como informes médicos,
/// denuncias, resoluciones, etc.
/// </summary>
public class TipoDocumentoIdentidad : BaseEntity
{
    /// <summary>
    /// Código técnico y estable.
    ///
    /// Ejemplos:
    /// DIP
    /// PASAPORTE
    /// OTRO
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nombre visible para el usuario.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }


    /// <summary>
    /// Personas que utilizan este tipo de documento.
    ///
    /// Relación:
    /// TipoDocumentoIdentidad 1 --- N Persona
    /// </summary>
    public ICollection<Persona> Personas { get; set; }
        = new List<Persona>();
}