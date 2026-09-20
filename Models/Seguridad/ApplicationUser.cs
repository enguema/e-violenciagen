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


    // =========================================================
    // DATOS PROPIOS DEL USUARIO
    // =========================================================

    /// <summary>
    /// Nombre real del titular de la cuenta.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;


    /// <summary>
    /// Apellidos del titular de la cuenta.
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;


    // =========================================================
    // CONTEXTO INSTITUCIONAL
    // =========================================================

    /// <summary>
    /// Institución a la que pertenece el usuario.
    ///
    /// Todo usuario interno del sistema deberá estar
    /// asociado a una institución.
    /// </summary>
    public Guid InstitucionId { get; set; }


    /// <summary>
    /// Unidad organizativa concreta dentro de la institución.
    ///
    /// Es opcional porque puede haber usuarios vinculados
    /// directamente a la institución sin una unidad específica.
    /// </summary>
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


    /// <summary>
    /// Momento en que fue creada la cuenta.
    /// Guardamos instantes en UTC.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;


    /// <summary>
    /// Último acceso correcto del usuario.
    ///
    /// Será null mientras el usuario nunca haya iniciado sesión.
    /// </summary>
    public DateTime? UltimoAcceso { get; set; }


    // =========================================================
    // NAVEGACIONES
    // =========================================================

    public Institucion Institucion { get; set; } = null!;

    public UnidadOrganizativa? UnidadOrganizativa { get; set; }
}