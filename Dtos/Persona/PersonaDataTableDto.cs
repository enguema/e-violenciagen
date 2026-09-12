namespace e_violenciagen.Dtos.Personas;

/// <summary>
/// Representa exclusivamente la información que necesita
/// el listado principal de personas.
///
/// Este DTO está pensado para ser utilizado por DataTables
/// en modo server-side, por lo que debe mantenerse ligero.
/// </summary>
public class PersonaDataTableDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    public bool Activo { get; set; }

    /// <summary>
    /// Ruta relativa de la fotografía.
    ///
    /// Ejemplo:
    /// /uploads/personas/65c0f2....jpg
    /// </summary>
    public string? RutaFoto { get; set; }


    /// <summary>
    /// Propiedad calculada para facilitar posteriormente
    /// la presentación en el DataTable.
    ///
    /// No existe como columna en la base de datos.
    /// </summary>
    public string NombreCompleto =>
        $"{Nombre} {Apellido}".Trim();
}