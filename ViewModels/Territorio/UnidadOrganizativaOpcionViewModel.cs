namespace e_violenciagen.ViewModels;
public class UnidadOrganizativaOpcionViewModel
{
    /// <summary>
    /// Representación mínima de una unidad organizativa
    /// para desplegables dependientes.
    /// </summary>
    public Guid Id { get; set; }

    public Guid InstitucionId { get; set; }

    public string Nombre { get; set; }
        = string.Empty;
}