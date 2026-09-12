namespace e_violenciagen.Dtos.Personas;

/// <summary>
/// Representación ligera de una Persona para listados.
///
/// Este DTO contiene solamente la información necesaria
/// para mostrar una fila del DataTable.
///
/// No debemos cargar una entidad Persona completa cuando
/// únicamente necesitamos unas pocas columnas.
/// </summary>
public class PersonaListItemDto
{
    public Guid Id { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo preparado para mostrar directamente
    /// en el listado.
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;


    // =========================================================
    // DOCUMENTACIÓN
    // =========================================================

    public string? TipoDocumento { get; set; }

    public string? NumeroDocumento { get; set; }


    // =========================================================
    // CONTACTO
    // =========================================================

    public string? Telefono { get; set; }

    public string? Email { get; set; }


    // =========================================================
    // UBICACIÓN
    // =========================================================

    public string? Barrio { get; set; }


    // =========================================================
    // FOTOGRAFÍA
    // =========================================================

    public string? RutaFoto { get; set; }
}