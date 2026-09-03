namespace e_violenciagen.Models;
/// <summary>
/// Representa un expediente o caso de violencia registrado
/// dentro de SIGEVIG.
///
/// Esta es una de las entidades centrales del sistema.
///
/// Un Caso podrá posteriormente relacionarse con:
/// - víctimas;
/// - presuntos agresores;
/// - actuaciones institucionales;
/// - documentos;
/// - medidas de protección;
/// - historial de estados;
/// etc.
/// </summary>
public class Caso : BaseEntity
{
    /// <summary>
    /// Identificador técnico interno.
    ///
    /// No debe confundirse con CodigoCaso, que será
    /// el identificador legible para los usuarios.
    /// </summary>
    //public Guid Id { get; set; } = Guid.NewGuid();


    // =========================================================
    // IDENTIFICACIÓN DEL CASO
    // =========================================================

    /// <summary>
    /// Código público/administrativo del expediente.
    ///
    /// Ejemplo:
    /// VG-2026-000001
    ///
    /// Su generación automática se implementará en el Service
    /// cuando desarrollemos el registro de casos.
    /// </summary>
    public string CodigoCaso { get; set; } = string.Empty;


    // =========================================================
    // FECHAS PRINCIPALES
    // =========================================================

    /// <summary>
    /// Fecha en la que el caso fue registrado oficialmente
    /// en SIGEVIG.
    ///
    /// Representa un instante, por lo que utilizamos UTC.
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;


    /// <summary>
    /// Fecha conocida o estimada en la que ocurrió el hecho.
    ///
    /// Utilizamos DateOnly porque inicialmente nos interesa
    /// la fecha civil, no necesariamente una hora exacta.
    ///
    /// Puede ser null cuando todavía no se conozca.
    /// </summary>
    public DateOnly? FechaHecho { get; set; }


    // =========================================================
    // ESTADO DEL CASO
    // =========================================================

    /// <summary>
    /// Estado actual del caso.
    ///
    /// Ejemplos futuros:
    /// - Registrado
    /// - En investigación
    /// - Judicializado
    /// - Cerrado
    /// etc.
    /// </summary>
    public Guid EstadoCasoId { get; set; }

    public EstadoCaso EstadoCaso { get; set; } = null!;


    // =========================================================
    // LOCALIZACIÓN DEL HECHO
    // =========================================================

    /// <summary>
    /// Barrio o localidad territorial donde ocurrió el hecho.
    ///
    /// Es nullable porque puede desconocerse inicialmente.
    ///
    /// Desde Barrio podemos llegar a Distrito y Provincia.
    /// </summary>
    public Guid? BarrioHechoId { get; set; }

    public Barrio? BarrioHecho { get; set; }


    /// <summary>
    /// Complemento textual de la ubicación.
    ///
    /// Ejemplo:
    /// "Vivienda situada cerca del mercado..."
    ///
    /// No sustituye a la clasificación territorial.
    /// </summary>
    public string? LugarDescripcion { get; set; }


    // =========================================================
    // INFORMACIÓN DESCRIPTIVA
    // =========================================================

    /// <summary>
    /// Resumen inicial del caso.
    ///
    /// Debe ser breve y facilitar su identificación.
    /// </summary>
    public string? Resumen { get; set; }


    /// <summary>
    /// Relato o descripción inicial de los hechos conocidos.
    ///
    /// No debe confundirse con declaraciones formales,
    /// diligencias policiales u otros documentos que tendrán
    /// su propio tratamiento posteriormente.
    /// </summary>
    public string? RelatoInicial { get; set; }


    // =========================================================
    // TIPOS DE VIOLENCIA
    // =========================================================

    /// <summary>
    /// Relación muchos-a-muchos entre Caso y TipoViolencia.
    ///
    /// Un caso puede presentar varios tipos de violencia.
    /// </summary>
    public ICollection<CasoTipoViolencia> TiposViolencia { get; set; }
        = new List<CasoTipoViolencia>();
}