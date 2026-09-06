namespace e_violenciagen.Models;
/// <summary>
/// Clasifica las actuaciones que las instituciones
/// pueden realizar dentro de un caso.
/// </summary>
public class TipoActuacion : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    /// <summary>
    /// Actuaciones clasificadas con este tipo.
    /// </summary>
    public ICollection<Actuacion> Actuaciones { get; set; }
        = new List<Actuacion>();
}