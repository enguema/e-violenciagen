namespace e_violenciagen.Data;
/// <summary>
/// Contiene los nombres oficiales de los roles base.
///
/// Centralizar los nombres evita escribir cadenas
/// como "Administrador" en muchos sitios diferentes.
/// </summary>
public static class RolesSistema
{
    public const string Administrador = "Administrador";

    public const string GestorCasos = "GestorCasos";

    public const string Supervisor = "Supervisor";

    public const string Consulta = "Consulta";


    /// <summary>
    /// Roles que deben existir desde la instalación inicial
    /// de la aplicación.
    /// </summary>
    public static readonly string[] Todos =
    [
        Administrador,
        GestorCasos,
        Supervisor,
        Consulta
    ];
}