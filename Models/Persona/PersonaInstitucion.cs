using e_violenciagen.Models;

/// <summary>
/// Representa la vinculación profesional o institucional
/// de una persona.
///
/// Ejemplo:
///
/// Persona: Juan N.
/// Institución: Policía Nacional
/// Unidad: Comisaría Central
/// Cargo: Inspector
///
/// Esta relación permite mantener separada la identidad
/// personal de la función institucional.
/// </summary>
public class PersonaInstitucion : BaseEntity
{
    // =========================================================
    // PERSONA
    // =========================================================

    public Guid PersonaId { get; set; }

    public Persona Persona { get; set; } = null!;


    // =========================================================
    // INSTITUCIÓN
    // =========================================================

    public Guid InstitucionId { get; set; }

    public Institucion Institucion { get; set; } = null!;


    // =========================================================
    // UNIDAD ORGANIZATIVA
    // =========================================================

    /// <summary>
    /// Unidad concreta en la que presta servicios.
    ///
    /// Puede ser null si solamente conocemos
    /// la institución principal.
    /// </summary>
    public Guid? UnidadOrganizativaId { get; set; }

    public UnidadOrganizativa? UnidadOrganizativa { get; set; }


    // =========================================================
    // DATOS PROFESIONALES
    // =========================================================

    /// <summary>
    /// Cargo o función que desempeña.
    ///
    /// Por ahora lo mantenemos como texto porque todavía
    /// no sabemos si será necesario un catálogo nacional
    /// de cargos.
    /// </summary>
    public string? Cargo { get; set; }


    /// <summary>
    /// Código profesional, número de funcionario,
    /// número de colegiado u otro identificador institucional.
    /// </summary>
    public string? CodigoProfesional { get; set; }


    // =========================================================
    // VIGENCIA
    // =========================================================

    /// <summary>
    /// Fecha desde la cual comenzó la vinculación.
    ///
    /// DateOnly es suficiente porque interesa el día,
    /// no la hora exacta.
    /// </summary>
    public DateOnly? FechaInicio { get; set; }


    /// <summary>
    /// Cuando tiene valor significa que la vinculación
    /// ya terminó.
    /// </summary>
    public DateOnly? FechaFin { get; set; }
}