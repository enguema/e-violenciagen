namespace e_violenciagen.Models;
public class TipoInstitucion: BaseEntity
{
    /// <summary>
    /// Código interno estable.
    /// Ejemplo: MINISTERIO, POLICIA, SANIDAD.
    /// </summary>

    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    /// <summary>
    /// Instituciones pertenecientes a este tipo.
    ///
    /// Relación:
    /// TipoInstitucion 1 --- N Institucion
    /// </summary>
    public ICollection<Institucion> Instituciones { get; set; }
        = new List<Institucion>();
}