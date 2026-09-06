namespace e_violenciagen.ViewModels;
/// <summary>
/// Representa una opción sencilla utilizada por los
/// desplegables de las vistas.
///
/// Evita enviar entidades completas de EF Core al navegador.
/// </summary>
public class OpcionCatalogoViewModel
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
}