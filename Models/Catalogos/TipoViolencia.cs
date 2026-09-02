namespace e_violenciagen.Models;
/// <summary>
/// Catálogo de tipos de violencia reconocidos por el sistema.
///
/// La lista definitiva deberá ajustarse al marco normativo
/// y a los protocolos institucionales aplicables.
/// </summary>
public class TipoViolencia : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
