namespace e_violenciagen.Models;
/// <summary>
/// Representa un documento incorporado al expediente electrónico
/// de un caso.
///
/// El archivo físico no se almacena directamente en esta entidad.
/// Aquí guardamos sus metadatos y la referencia necesaria
/// para poder localizarlo.
/// </summary>
public class DocumentoExpediente : BaseEntity
{
    //public Guid Id { get; set; } = Guid.NewGuid();


    // =========================================================
    // CASO
    // =========================================================

    /// <summary>
    /// Todo documento debe pertenecer a un caso.
    /// </summary>
    public Guid CasoId { get; set; }

    public Caso Caso { get; set; } = null!;


    // =========================================================
    // ACTUACIÓN
    // =========================================================

    /// <summary>
    /// Actuación que originó o a la que pertenece el documento.
    ///
    /// Es opcional porque algunos documentos pueden incorporarse
    /// directamente al expediente sin estar ligados a una
    /// actuación concreta.
    /// </summary>
    public Guid? ActuacionId { get; set; }

    public Actuacion? Actuacion { get; set; }


    // =========================================================
    // TIPO DE DOCUMENTO
    // =========================================================

    public Guid TipoDocumentoExpedienteId { get; set; }

    public TipoDocumentoExpediente TipoDocumentoExpediente { get; set; }
        = null!;


    // =========================================================
    // DATOS DEL ARCHIVO
    // =========================================================

    /// <summary>
    /// Nombre original proporcionado por el usuario.
    ///
    /// Ejemplo:
    /// Informe_Medico_Maria.pdf
    /// </summary>
    public string NombreOriginal { get; set; } = string.Empty;


    /// <summary>
    /// Nombre interno utilizado realmente en el almacenamiento.
    ///
    /// Debe generarse en el servidor y no depender del
    /// nombre enviado por el navegador.
    /// </summary>
    public string NombreAlmacenado { get; set; } = string.Empty;


    /// <summary>
    /// Ruta relativa del archivo dentro del almacenamiento.
    ///
    /// Ejemplo:
    /// casos/2026/VG-2026-000045/...
    ///
    /// No recomendamos guardar una ruta absoluta del servidor.
    /// </summary>
    public string RutaAlmacenamiento { get; set; } = string.Empty;


    /// <summary>
    /// Tipo MIME detectado o validado por el servidor.
    ///
    /// Ejemplo:
    /// application/pdf
    /// image/jpeg
    /// </summary>
    public string? TipoMime { get; set; }


    /// <summary>
    /// Extensión del archivo.
    ///
    /// Ejemplo:
    /// .pdf
    /// .jpg
    /// .png
    /// </summary>
    public string? Extension { get; set; }


    /// <summary>
    /// Tamaño del archivo expresado en bytes.
    /// </summary>
    public long TamanoBytes { get; set; }


    // =========================================================
    // INTEGRIDAD DEL ARCHIVO
    // =========================================================

    /// <summary>
    /// Hash criptográfico del archivo.
    ///
    /// Posteriormente podremos utilizar SHA-256 para detectar
    /// modificaciones o duplicados.
    /// </summary>
    public string? HashArchivo { get; set; }


    // =========================================================
    // INFORMACIÓN DOCUMENTAL
    // =========================================================
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }


    /// <summary>
    /// Fecha propia del documento, cuando existe.
    ///
    /// Ejemplo:
    /// fecha que aparece en un informe médico o resolución.
    /// </summary>
    public DateOnly? FechaDocumento { get; set; }


    /// <summary>
    /// Momento en que el documento fue incorporado
    /// al expediente electrónico.
    /// </summary>
    public DateTime FechaIncorporacion { get; set; } = DateTime.UtcNow;


    // =========================================================
    // CONFIDENCIALIDAD
    // =========================================================

    /// <summary>
    /// Indica que el documento debe recibir un tratamiento
    /// especialmente restringido.
    ///
    /// En esta fase únicamente almacenamos el indicador.
    /// Las reglas de acceso se implementarán cuando incorporemos
    /// autorización y permisos.
    /// </summary>
    public bool EsConfidencial { get; set; }
}