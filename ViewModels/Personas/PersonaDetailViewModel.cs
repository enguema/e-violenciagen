namespace  e_violenciagen.ViewModels.Personas;

/// <summary>
/// ViewModel utilizado exclusivamente por la vista
/// Details de Persona.
///
/// Contiene únicamente la información que necesita
/// la interfaz para presentar la ficha de una persona.
/// </summary>
public class PersonaDetailViewModel
{
    // =========================================================
    // IDENTIFICACIÓN INTERNA
    // =========================================================

    public Guid Id { get; set; }


    // =========================================================
    // IDENTIFICACIÓN PERSONAL
    // =========================================================

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
    // UBICACIÓN
    // =========================================================

    public string? Direccion { get; set; }

    public string? Barrio { get; set; }

    public string? Distrito { get; set; }

    public string? Provincia { get; set; }

    public int? Edad { get; set; }


    // =========================================================
    // RESUMEN DE PARTICIPACIÓN
    // =========================================================

    public int NumeroCasosComoVictima { get; set; }

    public int NumeroCasosComoPresuntoAgresor { get; set; }

    public int NumeroVinculacionesInstitucionales { get; set; }
}