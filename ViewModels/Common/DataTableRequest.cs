using Microsoft.AspNetCore.Mvc;

namespace e_violenciagen .ViewModels.Common;

/// <summary>
/// Representa los parámetros principales enviados por
/// jQuery DataTables cuando serverSide = true.
///
/// No contiene lógica de acceso a datos.
/// Solamente representa la petición HTTP.
/// </summary>
public class DataTableRequest
{
    /// <summary>
    /// Número utilizado por DataTables para relacionar
    /// cada petición AJAX con su respuesta.
    /// </summary>
    [FromForm(Name = "draw")]
    public int Draw { get; set; }


    /// <summary>
    /// Posición del primer registro solicitado.
    ///
    /// Ejemplo:
    /// Página 1 -> 0
    /// Página 2 -> 10
    /// Página 3 -> 20
    /// </summary>
    [FromForm(Name = "start")]
    public int Start { get; set; }


    /// <summary>
    /// Cantidad de registros solicitados.
    ///
    /// Normalmente:
    /// 10, 25, 50, etc.
    /// </summary>
    [FromForm(Name = "length")]
    public int Length { get; set; } = 10;


    /// <summary>
    /// Texto introducido en el buscador global
    /// del DataTable.
    /// </summary>
    [FromForm(Name = "search[value]")]
    public string? SearchValue { get; set; }


    /// <summary>
    /// Índice de la columna por la cual DataTables
    /// solicita ordenar.
    /// </summary>
    [FromForm(Name = "order[0][column]")]
    public int? OrderColumn { get; set; }


    /// <summary>
    /// Dirección solicitada:
    ///
    /// asc
    /// desc
    /// </summary>
    [FromForm(Name = "order[0][dir]")]
    public string? OrderDirection { get; set; }
}