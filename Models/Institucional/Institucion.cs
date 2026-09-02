namespace e_violenciagen.Models;

public class Institucion : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Siglas { get; set; }
    public string? Descripcion { get; set; }


    // =====================================================
    // RELACIÓN CON TIPO DE INSTITUCIÓN
    // =====================================================

    public Guid TipoInstitucionId { get; set; }

    public TipoInstitucion TipoInstitucion { get; set; } = null!;

    /// <summary>
    /// Personas vinculadas profesionalmente a esta institución.
    /// </summary>
    public ICollection<PersonaInstitucion> PersonasVinculadas { get; set; }
        = new List<PersonaInstitucion>();

}