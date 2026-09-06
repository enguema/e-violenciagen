namespace e_violenciagen.Models;
/// <summary>
/// Clasifica los documentos generados o incorporados
/// al expediente electrónico.
///
/// No debe confundirse con el tipo de documento
/// de identidad de una persona.
/// </summary>
public class TipoDocumentoExpediente : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    /// <summary>
    /// Documentos clasificados con este tipo.
    /// </summary>
    public ICollection<DocumentoExpediente> Documentos { get; set; }
        = new List<DocumentoExpediente>();
}