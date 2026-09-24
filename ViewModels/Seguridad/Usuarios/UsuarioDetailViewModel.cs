namespace e_violenciagen.ViewModels;
public class UsuarioDetailViewModel
{
    public Guid Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Institucion { get; set; } = string.Empty;

    public string? UnidadOrganizativa { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public List<string> Roles { get; set; } = [];
}