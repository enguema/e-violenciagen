using Microsoft.AspNetCore.Authorization;

namespace e_violenciagen.Seguridad;

/// <summary>
/// Requisito de autorización que indica el permiso
/// que debe poseer el usuario.
///
/// Ejemplo:
///
/// CASOS_CREAR
/// PERSONAS_EDITAR
/// DOCUMENTOS_SUBIR
/// </summary>
public sealed class PermisoRequirement : IAuthorizationRequirement
{
    public PermisoRequirement(string codigoPermiso)
    {
        if (string.IsNullOrWhiteSpace(codigoPermiso))
        {
            throw new ArgumentException(
                "El código del permiso es obligatorio.",
                nameof(codigoPermiso));
        }

        CodigoPermiso = codigoPermiso;
    }


    /// <summary>
    /// Código técnico del permiso que debe tener
    /// el usuario para superar la Policy.
    /// </summary>
    public string CodigoPermiso { get; }
}