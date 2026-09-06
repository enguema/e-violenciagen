namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Representa una fila del listado general de expedientes.
///
/// No devolvemos directamente la entidad Caso a la vista.
/// La vista recibe únicamente la información que necesita.
/// </summary>
public class CasoIndexItemViewModel
{
    public Guid Id { get; set; }

    public string CodigoCaso { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public DateOnly? FechaHecho { get; set; }

    public string Estado { get; set; } = string.Empty;

    public int NumeroVictimas { get; set; }

    public int NumeroPresuntosAgresores { get; set; }

    public int NumeroActuaciones { get; set; }

    public int NumeroDocumentos { get; set; }
}