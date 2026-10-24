using Microsoft.AspNetCore.Identity;
namespace e_violenciagen.Models;
/// <summary>
/// Representa una cuenta que puede autenticarse
/// y utilizar la aplicación.
/// 
/// IMPORTANTE:
/// ApplicationUser NO representa una Persona
/// implicada en un caso de violencia de género.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /*
     * IdentityUser ya incluye muchas propiedades:
     *
     * Id
     * UserName
     * NormalizedUserName
     * Email
     * NormalizedEmail
     * EmailConfirmed
     * PasswordHash
     * SecurityStamp
     * PhoneNumber
     * PhoneNumberConfirmed
     * TwoFactorEnabled
     * LockoutEnd
     * LockoutEnabled
     * AccessFailedCount
     *
     * Por eso NO debemos volver a declarar esas propiedades.
     */

    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public Guid InstitucionId { get; set; }
    public Guid? UnidadOrganizativaId { get; set; }


    // =========================================================
    // ESTADO FUNCIONAL
    // =========================================================

    /// <summary>
    /// Indica si la cuenta está administrativamente activa.
    ///
    /// Es diferente del bloqueo automático de Identity.
    ///
    /// Activo = decisión administrativa.
    /// LockoutEnd = bloqueo de seguridad gestionado por Identity.
    /// </summary>
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcceso { get; set; }


    // =========================================================
    // NAVEGACIONES
    // =========================================================

    public Institucion Institucion { get; set; } = null!;

    public UnidadOrganizativa? UnidadOrganizativa { get; set; }
}