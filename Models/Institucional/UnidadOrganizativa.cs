using e_violenciagen.Models;

namespace e_violenciagen.Models;

/// <summary>
/// Representa una dependencia perteneciente a una institución.
///
/// Puede representar:
/// - Dirección General
/// - Departamento
/// - Comisaría
/// - Hospital
/// - Centro de Salud
/// - Fiscalía territorial
/// - Juzgado
/// etc.
/// </summary>
public class UnidadOrganizativa: BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    // =====================================================
    // INSTITUCIÓN A LA QUE PERTENECE
    // =====================================================

    public Guid InstitucionId { get; set; }

    public Institucion Institucion { get; set; } = null!;

    /// <summary>
    /// Unidad superior.
    ///
    /// Es nullable porque una unidad de primer nivel
    /// puede no depender de otra unidad.
    /// </summary>
    public Guid? UnidadPadreId { get; set; }

    public UnidadOrganizativa? UnidadPadre { get; set; }


    /// <summary>
    /// Unidades que dependen directamente de esta unidad.
    /// </summary>
    public ICollection<UnidadOrganizativa> UnidadesHijas { get; set; }
        = new List<UnidadOrganizativa>();
}