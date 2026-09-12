namespace e_violenciagen.Dtos.Common;

/// <summary>
/// Estructura de respuesta esperada por DataTables
/// cuando funciona en modo serverSide.
/// </summary>
public class DataTableResponse<T>
{
    /// <summary>
    /// Debe devolver el mismo draw recibido en la petición.
    /// </summary>
    public int Draw { get; set; }


    /// <summary>
    /// Número total de registros existentes en la tabla,
    /// sin aplicar búsqueda.
    /// </summary>
    public int RecordsTotal { get; set; }


    /// <summary>
    /// Número de registros que cumplen el filtro actual.
    /// </summary>
    public int RecordsFiltered { get; set; }


    /// <summary>
    /// Únicamente los registros correspondientes
    /// a la página actual.
    /// </summary>
    public IReadOnlyList<T> Data { get; set; }
        = Array.Empty<T>();
}