namespace e_violenciagen.Models;
/// <summary>
/// Representa la participación de una Persona como víctima
/// dentro de un Caso concreto.
///
/// Importante:
/// La persona no queda marcada permanentemente como víctima.
/// Su condición de víctima existe únicamente en relación
/// con el caso indicado.
/// </summary>
public class CasoVictima
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
    // DATOS PROPIOS DE LA PARTICIPACIÓN
    // =========================================================

    /// <summary>
    /// Indica si se considera la víctima principal del caso.
    ///
    /// Un caso puede tener varias víctimas, pero normalmente
    /// puede existir una persona que origine o represente
    /// principalmente el expediente.
    /// </summary>
    public bool EsVictimaPrincipal { get; set; }


    /// <summary>
    /// Permite señalar inicialmente que la persona requiere
    /// una atención especial o valoración de protección.
    ///
    /// No sustituye a futuras entidades específicas de
    /// valoración de riesgo o medidas de protección.
    /// </summary>
    public bool RequiereProteccion { get; set; }


    /// <summary>
    /// Información adicional sobre la participación
    /// de esta víctima en el caso.
    ///
    /// Debe evitarse almacenar aquí información clínica
    /// o extremadamente sensible que posteriormente tenga
    /// un módulo específico.
    /// </summary>
    public string? Observaciones { get; set; }


    /// <summary>
    /// Fecha en la que la persona fue vinculada
    /// al expediente como víctima.
    /// </summary>
    public DateTime FechaVinculacion { get; set; } = DateTime.UtcNow;
}