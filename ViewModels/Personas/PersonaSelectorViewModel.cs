namespace e_violenciagen.ViewModels.Personas;

/// <summary>
/// Configuración utilizada por el parcial reutilizable
/// _PersonaSelector.
///
/// Permite utilizar el mismo componente para víctima,
/// presunto agresor u otros formularios.
/// </summary>
public class PersonaSelectorViewModel
{
    /// <summary>
    /// Nombre del campo hidden que contendrá PersonaId.
    ///
    /// Ejemplo:
    /// VictimaPersonaId
    /// PresuntoAgresorPersonaId
    /// </summary>
    public string FieldName { get; set; } = string.Empty;


    /// <summary>
    /// Texto visible encima del buscador.
    /// </summary>
    public string Label { get; set; } = "Persona";


    /// <summary>
    /// Texto orientativo dentro del buscador.
    /// </summary>
    public string Placeholder { get; set; } = "Buscar por nombre o documento...";


    /// <summary>
    /// Persona previamente seleccionada.
    ///
    /// Será útil especialmente en formularios Edit.
    /// </summary>
    public Guid? SelectedPersonaId { get; set; }


    public string? SelectedPersonaName { get; set; }
}