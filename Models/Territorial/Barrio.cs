namespace e_violenciagen.Models;
public class Barrio: BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public Guid DistritoId { get; set; }

    public Distrito Distrito { get; set; } = null!;
}