namespace e_violenciagen.Dtos.Personas;

/// <summary>
/// Información utilizada para mostrar la ficha detallada
/// de una persona.
///
/// Separamos este DTO del utilizado por el DataTable porque
/// la pantalla Details necesita más información que un listado.
/// </summary>
public class PersonaDetailsDto
{
    public Guid Id { get; set; }


    // =========================================================
    // IDENTIFICACIÓN
    // =========================================================

    public Guid? TipoDocumentoIdentidadId { get; set; }

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }


    // =========================================================
    // DATOS PERSONALES
    // =========================================================

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public DateOnly? FechaNacimiento { get; set; }

    public string? Sexo { get; set; }

    public string? RutaFoto { get; set; }


    // =========================================================
    // CONTACTO
    // =========================================================

    public string? Telefono { get; set; }

    public string? Email { get; set; }


    // =========================================================
    // DIRECCIÓN
    // =========================================================

    public string? Direccion { get; set; }

    public Guid? BarrioId { get; set; }

    public string? Barrio { get; set; }

    public string? Distrito { get; set; }

    public string? Provincia { get; set; }


    // =========================================================
    // INFORMACIÓN RELACIONADA
    // =========================================================

    /// <summary>
    /// Cantidad de casos en los que la persona figura
    /// como víctima.
    /// </summary>
    public int NumeroCasosComoVictima { get; set; }


    /// <summary>
    /// Cantidad de casos en los que figura como
    /// presunto agresor.
    /// </summary>
    public int NumeroCasosComoPresuntoAgresor { get; set; }


    /// <summary>
    /// Número de vinculaciones institucionales registradas.
    /// </summary>
    public int NumeroVinculacionesInstitucionales { get; set; }
}