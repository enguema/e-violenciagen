namespace e_violenciagen.Models;
/// <summary>
/// Entidad intermedia que materializa la relación
/// muchos-a-muchos entre ApplicationRole y Permiso.
/// </summary>
public class RolPermiso
{
    /// <summary>
    /// Rol al que se concede el permiso.
    /// </summary>
    public Guid RolId { get; set; }


    /// <summary>
    /// Permiso concedido al rol.
    /// </summary>
    public Guid PermisoId { get; set; }


    // =========================================================
    // NAVEGACIONES
    // =========================================================

    public ApplicationRole Rol { get; set; } = null!;

    public Permiso Permiso { get; set; } = null!;
}