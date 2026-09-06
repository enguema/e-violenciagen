namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Resultado mínimo que el servidor devuelve al navegador
/// después de registrar correctamente un expediente.
/// </summary>
public class CasoCreateResultViewModel
{
    public Guid Id { get; set; }

    public string CodigoCaso { get; set; } = string.Empty;
}