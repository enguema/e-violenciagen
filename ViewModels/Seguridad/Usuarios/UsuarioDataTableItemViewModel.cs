namespace e_violenciagen.ViewModels;

public class UsuarioDataTableItemViewModel
{
    public Guid Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Institucion { get; set; } = string.Empty;

    public string UnidadOrganizativa { get; set; } = string.Empty;

    public string Roles { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public bool Bloqueado { get; set; }

    public DateTime? UltimoAcceso { get; set; }
}