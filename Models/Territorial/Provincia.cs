namespace e_violenciagen.Models;

public class Provincia: BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Distrito> Distritos { get; set; }
        = new List<Distrito>();
}
