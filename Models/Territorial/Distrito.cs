namespace e_violenciagen.Models;
public class Distrito: BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public Guid ProvinciaId { get; set; }

    public Provincia Provincia { get; set; } = null!;

    public ICollection<Barrio> Barrios { get; set; }
        = new List<Barrio>();
}