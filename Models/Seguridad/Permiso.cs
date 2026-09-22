namespace e_violenciagen.Models;
/// <summary>
/// Representa una capacidad concreta dentro del sistema.
///
/// Ejemplos:
///
/// CASOS_VER
/// CASOS_CREAR
/// CASOS_EDITAR
/// USUARIOS_GESTIONAR
///
/// Un permiso no representa un rol.
/// Los roles agrupan varios permisos.
/// </summary>
public class Permiso
{
    public Guid Id { get; set; }


    /// <summary>
    /// Código técnico y estable utilizado internamente
    /// para identificar el permiso.
    ///
    /// Ejemplo:
    /// CASOS_CREAR
    ///
    /// Debe ser único.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;


    /// <summary>
    /// Nombre legible para mostrar al administrador.
    ///
    /// Ejemplo:
    /// "Crear casos"
    /// </summary>
    public string Nombre { get; set; } = string.Empty;


    /// <summary>
    /// Explicación funcional del permiso.
    /// </summary>
    public string? Descripcion { get; set; }


    /// <summary>
    /// Permite agrupar permisos visualmente
    /// y funcionalmente por módulo.
    ///
    /// Ejemplos:
    ///
    /// CASOS
    /// PERSONAS
    /// ACTUACIONES
    /// DOCUMENTOS
    /// SEGURIDAD
    /// </summary>
    public string Modulo { get; set; } = string.Empty;


    /// <summary>
    /// Permite desactivar un permiso sin eliminarlo.
    /// </summary>
    public bool Activo { get; set; } = true;


    // =========================================================
    // NAVEGACIONES
    // =========================================================

    /// <summary>
    /// Roles que tienen asignado este permiso.
    /// </summary>
    public ICollection<RolPermiso> RolPermisos { get; set; }
        = new List<RolPermiso>();
}