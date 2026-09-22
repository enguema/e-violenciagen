using Microsoft.AspNetCore.Identity;
namespace e_violenciagen.Models;
/// <summary>
/// Representa un rol de seguridad dentro del sistema.
///
/// Ejemplos futuros:
///
/// Administrador
/// GestorCasos
/// Supervisor
/// Consulta
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /*
     * IdentityRole ya incluye:
     *
     * Id
     * Name
     * NormalizedName
     * ConcurrencyStamp
     *
     * Por tanto, tampoco debemos volver
     * a declarar estas propiedades.
     */


    /// <summary>
    /// Explicación funcional del rol.
    ///
    /// Ejemplo:
    /// "Permite registrar, consultar y actualizar casos".
    /// </summary>
    public string? Descripcion { get; set; }


    /// <summary>
    /// Permite desactivar un rol sin eliminarlo.
    /// </summary>
    public bool Activo { get; set; } = true;


    /// <summary>
    /// Identifica roles fundamentales del sistema.
    ///
    /// Posteriormente podremos evitar, por ejemplo,
    /// que el rol Administrador sea eliminado.
    /// </summary>
    public bool EsSistema { get; set; } = false;

    /*
     * Relación N:N con Permiso
     * mediante RolPermiso.
     */
    public ICollection<RolPermiso> RolPermisos { get; set; }
        = new List<RolPermiso>();
}