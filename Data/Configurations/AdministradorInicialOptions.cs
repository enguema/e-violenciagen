namespace e_violenciagen.Data;
/// <summary>
/// Configuración necesaria para crear el primer
/// administrador de la aplicación.
/// </summary>
public class AdministradorInicialOptions
{
    public const string SectionName = "AdministradorInicial";


    public string NombreUsuario { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;


    /*
     * La institución debe existir previamente
     * en nuestra base de datos.
     
    public Guid InstitucionId { get; set; }
    */

    /*
     * En vez de guardar un Guid de base de datos,
     * identificamos la institución mediante
     * un código estable.
     *
     * Ejemplo:
     * MASIG
     */
    public string CodigoInstitucion { get; set; }
        = string.Empty;


    /*
     * Nunca debemos almacenar esta contraseña
     * directamente en el repositorio Git.
     * La contraseña vendrá de User Secrets o de una variable de entorno.
     */
    public string Password { get; set; } = string.Empty;
}