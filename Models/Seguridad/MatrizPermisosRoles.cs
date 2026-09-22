using e_violenciagen.Data;

namespace e_violenciagen.Models;
/// <summary>
/// Define la asignación inicial de permisos
/// a los roles base.
///
/// Esta clase no consulta la base de datos.
/// Solo describe la matriz inicial de seguridad.
/// </summary>
public static class MatrizPermisosRoles
{
    public static readonly IReadOnlyDictionary<string, string[]>
        PorRol =
        new Dictionary<string, string[]>
        {
            // =================================================
            // ADMINISTRADOR
            // =================================================

            /*
             * El Administrador recibe todos los permisos
             * definidos actualmente.
             *
             * En el Seeder resolveremos esta asignación
             * dinámicamente a partir de PermisosSistema.Todos.
             */
            [RolesSistema.Administrador] = [],


            // =================================================
            // GESTOR DE CASOS
            // =================================================

            [RolesSistema.GestorCasos] =
            [
                PermisosSistema.Personas.Ver,
                PermisosSistema.Personas.Crear,
                PermisosSistema.Personas.Editar,

                PermisosSistema.Casos.Ver,
                PermisosSistema.Casos.Crear,
                PermisosSistema.Casos.Editar,

                PermisosSistema.Actuaciones.Ver,
                PermisosSistema.Actuaciones.Crear,

                PermisosSistema.Documentos.Ver,
                PermisosSistema.Documentos.Subir
            ],


            // =================================================
            // SUPERVISOR
            // =================================================

            [RolesSistema.Supervisor] =
            [
                PermisosSistema.Personas.Ver,

                PermisosSistema.Casos.Ver,
                PermisosSistema.Casos.Editar,
                PermisosSistema.Casos.Cerrar,

                PermisosSistema.Actuaciones.Ver,

                PermisosSistema.Documentos.Ver
            ],


            // =================================================
            // CONSULTA
            // =================================================

            [RolesSistema.Consulta] =
            [
                PermisosSistema.Personas.Ver,
                PermisosSistema.Casos.Ver,
                PermisosSistema.Actuaciones.Ver,
                PermisosSistema.Documentos.Ver
            ]
        };
}