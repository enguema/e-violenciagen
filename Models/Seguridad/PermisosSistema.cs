namespace e_violenciagen.Models;
/// <summary>
/// Códigos oficiales de permisos utilizados
/// por la aplicación.
///
/// Los valores deben coincidir con los códigos
/// almacenados en la base de datos.
/// </summary>
public static class PermisosSistema
{
    // =========================================================
    // PERSONAS
    // =========================================================

    public static class Personas
    {
        public const string Ver = "PERSONAS_VER";
        public const string Crear = "PERSONAS_CREAR";
        public const string Editar = "PERSONAS_EDITAR";
    }


    // =========================================================
    // CASOS
    // =========================================================

    public static class Casos
    {
        public const string Ver = "CASOS_VER";
        public const string Crear = "CASOS_CREAR";
        public const string Editar = "CASOS_EDITAR";
        public const string Cerrar = "CASOS_CERRAR";
    }


    // =========================================================
    // ACTUACIONES
    // =========================================================

    public static class Actuaciones
    {
        public const string Ver = "ACTUACIONES_VER";
        public const string Crear = "ACTUACIONES_CREAR";
    }


    // =========================================================
    // DOCUMENTOS
    // =========================================================

    public static class Documentos
    {
        public const string Ver = "DOCUMENTOS_VER";
        public const string Subir = "DOCUMENTOS_SUBIR";
    }


    // =========================================================
    // SEGURIDAD
    // =========================================================

    public static class Seguridad
    {
        public const string UsuariosVer = "USUARIOS_VER";
        public const string UsuariosCrear = "USUARIOS_CREAR";
        public const string UsuariosEditar = "USUARIOS_EDITAR";
        public const string RolesGestionar = "ROLES_GESTIONAR";
    }


    // =========================================================
    // DEFINICIONES PARA EL SEEDER
    // =========================================================

    public static readonly IReadOnlyCollection<DefinicionPermiso> Todos =
    [
        // PERSONAS
        new(
            Personas.Ver,
            "Consultar personas",
            "PERSONAS",
            "Permite consultar las personas registradas."),

        new(
            Personas.Crear,
            "Crear personas",
            "PERSONAS",
            "Permite registrar nuevas personas."),

        new(
            Personas.Editar,
            "Editar personas",
            "PERSONAS",
            "Permite modificar datos de personas existentes."),


        // CASOS
        new(
            Casos.Ver,
            "Consultar casos",
            "CASOS",
            "Permite consultar casos registrados."),

        new(
            Casos.Crear,
            "Crear casos",
            "CASOS",
            "Permite registrar nuevos casos."),

        new(
            Casos.Editar,
            "Editar casos",
            "CASOS",
            "Permite modificar información de casos."),

        new(
            Casos.Cerrar,
            "Cerrar casos",
            "CASOS",
            "Permite cerrar formalmente un caso."),


        // ACTUACIONES
        new(
            Actuaciones.Ver,
            "Consultar actuaciones",
            "ACTUACIONES",
            "Permite consultar actuaciones registradas."),

        new(
            Actuaciones.Crear,
            "Crear actuaciones",
            "ACTUACIONES",
            "Permite registrar actuaciones institucionales."),


        // DOCUMENTOS
        new(
            Documentos.Ver,
            "Consultar documentos",
            "DOCUMENTOS",
            "Permite consultar documentos del expediente."),

        new(
            Documentos.Subir,
            "Subir documentos",
            "DOCUMENTOS",
            "Permite incorporar documentos al expediente."),


        // SEGURIDAD
        new(
            Seguridad.UsuariosVer,
            "Consultar usuarios",
            "SEGURIDAD",
            "Permite consultar las cuentas de usuario."),

        new(
            Seguridad.UsuariosCrear,
            "Crear usuarios",
            "SEGURIDAD",
            "Permite crear nuevas cuentas de usuario."),

        new(
            Seguridad.UsuariosEditar,
            "Editar usuarios",
            "SEGURIDAD",
            "Permite modificar cuentas existentes."),

        new(
            Seguridad.RolesGestionar,
            "Gestionar roles",
            "SEGURIDAD",
            "Permite administrar roles y sus permisos.")
    ];

    
}