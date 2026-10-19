namespace e_violenciagen.ViewModels;
public class ParticipanteOpcionViewModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? UnidadOrganizativa { get; set; }
}