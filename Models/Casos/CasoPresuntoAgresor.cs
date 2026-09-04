namespace e_violenciagen.Models;
/// <summary>
/// Representa la participación de una Persona como
/// presunto agresor dentro de un Caso.
///
/// El término "presunto" es importante:
/// SIGEVIG registra la situación conocida en el expediente,
/// no establece por sí mismo responsabilidad penal.
/// </summary>
public class CasoPresuntoAgresor
{
    // =========================================================
    // CASO
    // =========================================================

    public Guid CasoId { get; set; }

    public Caso Caso { get; set; } = null!;


    // =========================================================
    // PERSONA
    // =========================================================

    public Guid PersonaId { get; set; }

    public Persona Persona { get; set; } = null!;


    // =========================================================
    // INFORMACIÓN DE LA RELACIÓN
    // =========================================================

    /// <summary>
    /// Relación conocida entre el presunto agresor
    /// y la víctima o víctimas.
    ///
    /// Por ahora usamos texto porque todavía no hemos
    /// creado un catálogo oficial de relaciones.
    ///
    /// Ejemplos:
    /// Pareja
    /// Expareja
    /// Cónyuge
    /// Familiar
    /// Conocido
    /// etc.
    /// </summary>
    public string? RelacionConVictima { get; set; }


    /// <summary>
    /// Información adicional relativa al papel del
    /// presunto agresor dentro del caso.
    /// </summary>
    public string? Observaciones { get; set; }


    /// <summary>
    /// Momento en el que la persona fue vinculada
    /// al caso como presunto agresor.
    /// </summary>
    public DateTime FechaVinculacion { get; set; } = DateTime.UtcNow;
}