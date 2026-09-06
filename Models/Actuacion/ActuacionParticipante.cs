namespace e_violenciagen.Models;
/// <summary>
/// Relaciona una actuación con los profesionales
/// institucionales que participaron en ella.
///
/// Utilizamos PersonaInstitucion y no Persona directamente,
/// porque nos interesa conservar el contexto profesional
/// dentro del cual intervino la persona.
/// </summary>
public class ActuacionParticipante
{
    // =========================================================
    // ACTUACIÓN
    // =========================================================

    public Guid ActuacionId { get; set; }

    public Actuacion Actuacion { get; set; } = null!;


    // =========================================================
    // PERSONA / INSTITUCIÓN
    // =========================================================

    public Guid PersonaInstitucionId { get; set; }

    public PersonaInstitucion PersonaInstitucion { get; set; } = null!;


    // =========================================================
    // FUNCIÓN DENTRO DE LA ACTUACIÓN
    // =========================================================

    /// <summary>
    /// Función concreta desempeñada dentro de la actuación.
    ///
    /// Ejemplos:
    /// - Responsable.
/// - Médico.
/// - Psicólogo.
/// - Inspector.
/// - Fiscal.
/// - Secretario.
///
/// Por ahora se mantiene como texto.
/// Si posteriormente necesitamos estadísticas normalizadas,
/// podremos convertirlo en un catálogo.
/// </summary>
    public string? RolEnActuacion { get; set; }


    /// <summary>
    /// Permite identificar al profesional principal
    /// responsable de la actuación.
    /// </summary>
    public bool EsResponsable { get; set; }
}