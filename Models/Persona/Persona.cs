namespace e_violenciagen.Models;
/// <summary>
/// Representa a una persona física conocida por SIGEVIG.
///
/// La entidad Persona NO indica todavía si alguien es:
/// - víctima;
/// - presunto agresor;
/// - funcionario;
/// - médico;
/// - fiscal;
/// - juez.
///
/// Esas condiciones se representarán mediante relaciones
/// específicas dependiendo del contexto.
/// </summary>
public class Persona : BaseEntity
{
    // =========================================================
    // IDENTIFICACIÓN
    // =========================================================

    /// <summary>
    /// Tipo de documento de identidad.
    ///
    /// Es nullable porque puede existir una persona
    /// cuyo documento todavía no esté disponible.
    /// </summary>
    public Guid? TipoDocumentoIdentidadId { get; set; }

    public TipoDocumentoIdentidad? TipoDocumentoIdentidad { get; set; }


    /// <summary>
    /// Número de documento.
    ///
    /// También puede ser nulo cuando la persona
    /// no disponga de documentación conocida.
    /// </summary>
    public string? NumeroDocumento { get; set; }


    // =========================================================
    // DATOS PERSONALES BÁSICOS
    // =========================================================
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;


    /// <summary>
    /// Fecha de nacimiento.
    ///
    /// Utilizamos DateOnly porque representa una fecha civil,
    /// no un instante exacto del tiempo.
    /// </summary>
    public DateOnly? FechaNacimiento { get; set; }


    /// <summary>
    /// Sexo registrado para fines administrativos/estadísticos.
    ///
    /// En esta primera versión utilizamos una cadena limitada.
    /// Si posteriormente los requisitos normativos exigen
    /// un catálogo específico, podremos extraerlo.
    /// </summary>
    public string? Sexo { get; set; }


    // =========================================================
    // CONTACTO
    // =========================================================
    public string? Telefono { get; set; }
    public string? Email { get; set; }


    // =========================================================
    // DIRECCIÓN
    // =========================================================
    public string? Direccion { get; set; }

    /// <summary>
    /// Barrio de residencia, cuando se conoce.
    ///
    /// Desde Barrio podremos navegar hacia Distrito
    /// y posteriormente hacia Provincia.
    /// </summary>
    public Guid? BarrioId { get; set; }

    public Barrio? Barrio { get; set; }


    // =========================================================
    // VINCULACIONES INSTITUCIONALES
    // =========================================================

    /// <summary>
    /// Una persona puede haber trabajado o colaborado
    /// con una o varias instituciones a lo largo del tiempo.
    /// </summary>
    public ICollection<PersonaInstitucion> VinculacionesInstitucionales
    { get; set; } = new List<PersonaInstitucion>();

    // =========================================================
    // PARTICIPACIÓN EN CASOS
    // =========================================================

    /// <summary>
    /// Casos en los que esta persona figura como víctima.
    ///
    /// Esto NO significa que Persona sea una "entidad víctima";
    /// únicamente muestra las relaciones concretas existentes.
    /// </summary>
    public ICollection<CasoVictima> CasosComoVictima { get; set; }
        = new List<CasoVictima>();


    /// <summary>
    /// Casos en los que esta persona figura como
    /// presunto agresor.
    /// </summary>
    public ICollection<CasoPresuntoAgresor> CasosComoPresuntoAgresor
    { get; set; } = new List<CasoPresuntoAgresor>();
}