namespace e_violenciagen.Models;

/// <summary>
/// Representa una actuación realizada por una institución
/// dentro del seguimiento de un caso.
///
/// Ejemplos:
/// - Recepción de denuncia.
/// - Atención médica.
/// - Evaluación social.
/// - Diligencia policial.
/// - Actuación fiscal.
/// - Resolución judicial.
///
/// Una actuación siempre pertenece a un único Caso.
/// </summary>
public class Actuacion : BaseEntity
{
    // =========================================================
    // CASO
    // =========================================================

    /// <summary>
    /// Caso sobre el que se realiza la actuación.
    /// </summary>
    public Guid CasoId { get; set; }

    public Caso Caso { get; set; } = null!;


    // =========================================================
    // TIPO DE ACTUACIÓN
    // =========================================================

    /// <summary>
    /// Clasifica qué clase de intervención se realizó.
    /// </summary>
    public Guid TipoActuacionId { get; set; }

    public TipoActuacion TipoActuacion { get; set; } = null!;


    // =========================================================
    // INSTITUCIÓN
    // =========================================================

    /// <summary>
    /// Institución responsable de la actuación.
    ///
    /// Ejemplos:
    /// Policía, Gendarmería, Sanidad, Fiscalía,
    /// Juzgados o Ministerio.
    /// </summary>
    public Guid InstitucionId { get; set; }

    public Institucion Institucion { get; set; } = null!;


    /// <summary>
    /// Unidad concreta que realizó la actuación.
    ///
    /// Es opcional porque puede conocerse únicamente
    /// la institución principal.
    /// </summary>
    public Guid? UnidadOrganizativaId { get; set; }

    public UnidadOrganizativa? UnidadOrganizativa { get; set; }


    // =========================================================
    // FECHA
    // =========================================================

    /// <summary>
    /// Momento real en el que ocurrió la actuación.
    ///
    /// No debe confundirse con FechaCreacion, que representa
    /// cuándo fue insertado el registro en SIGEVIG.
    /// </summary>
    public DateTime FechaActuacion { get; set; }


    // =========================================================
    // CONTENIDO
    // =========================================================

    /// <summary>
    /// Título o resumen breve de la actuación.
    ///
    /// Ejemplo:
    /// "Atención inicial en urgencias".
    /// </summary>
    public string Titulo { get; set; } = string.Empty;


    /// <summary>
    /// Descripción detallada de la actuación realizada.
    ///
    /// No debe utilizarse para almacenar documentos completos.
    /// Los documentos tendrán su propia entidad posteriormente.
    /// </summary>
    public string? Descripcion { get; set; }


    /// <summary>
    /// Resultado principal obtenido de la actuación.
    ///
    /// Ejemplo:
    /// "Expediente remitido a Fiscalía".
    ///
    /// Es opcional porque no todas las actuaciones
    /// producen un resultado formal inmediato.
    /// </summary>
    public string? Resultado { get; set; }


    // =========================================================
    // PARTICIPANTES
    // =========================================================

    /// <summary>
    /// Profesionales institucionales que participaron
    /// en esta actuación.
    /// </summary>
    public ICollection<ActuacionParticipante> Participantes { get; set; }
        = new List<ActuacionParticipante>();

    /// <summary>
    /// Documentos producidos o incorporados como parte
    /// de esta actuación.
    ///
    /// Una actuación puede no tener documentos.
    /// </summary>
    public ICollection<DocumentoExpediente> Documentos { get; set; }
        = new List<DocumentoExpediente>();
}