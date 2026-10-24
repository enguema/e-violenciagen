namespace e_violenciagen.ViewModels.Casos;

public class ActuacionParticipanteViewModel
{
    /// <summary>
    /// Id de la relación ActuacionParticipante.
    /// Nos sirve también para generar IDs únicos en los modales.
    /// </summary>
    public Guid Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string? UnidadOrganizativa { get; set; }
}