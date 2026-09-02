namespace e_violenciagen.Models;
/// <summary>
/// Catálogo que define los estados posibles dentro
/// del ciclo de vida de un caso.
///
/// La definición definitiva de estados podrá evolucionar
/// cuando analicemos los procedimientos reales.
/// </summary>
public class EstadoCaso : BaseEntity
{    
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    /// <summary>
    /// Permite controlar el orden en que los estados
    /// aparecerán en interfaces y listados.
    /// </summary>
    public int Orden { get; set; }
}