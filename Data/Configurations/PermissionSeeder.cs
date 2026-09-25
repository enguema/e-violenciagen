using e_violenciagen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace e_violenciagen.Data;
/// <summary>
/// Inicializa:
///
/// - permisos base;
/// - relaciones RolPermiso.
///
/// Debe poder ejecutarse múltiples veces
/// sin duplicar registros.
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope =
            services.CreateScope();


        AppDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();


        RoleManager<ApplicationRole> roleManager =
            scope.ServiceProvider
                .GetRequiredService<
                    RoleManager<ApplicationRole>>();


        // =====================================================
        // 1. CREAR PERMISOS
        // =====================================================

        await CrearPermisosAsync(dbContext);


        // =====================================================
        // 2. ASIGNAR PERMISOS A ROLES
        // =====================================================

        await AsignarPermisosARolesAsync(
            dbContext,
            roleManager);
    }


    private static async Task CrearPermisosAsync(AppDbContext dbContext)
    {
        /*
         * Recuperamos de una sola vez todos los códigos
         * ya existentes.
         *
         * Esto evita lanzar una consulta SQL
         * por cada permiso.
         */
        Dictionary<string, Permiso> existentes =
         await dbContext.Permisos
             .ToDictionaryAsync(
                 p => p.Codigo,
                 p => p);

        foreach (
            DefinicionPermiso definicion
            in PermisosSistema.Todos)
        {
            if (existentes.TryGetValue(
                definicion.Codigo,
                out Permiso? permisoExistente))
            {
                permisoExistente.Nombre =
                    definicion.Nombre;

                permisoExistente.Modulo =
                    definicion.Modulo;

                permisoExistente.Descripcion =
                    definicion.Descripcion;

                continue;
            }

            dbContext.Permisos.Add(
                new Permiso
                {
                    Id = Guid.NewGuid(),
                    Codigo = definicion.Codigo,
                    Nombre = definicion.Nombre,
                    Modulo = definicion.Modulo,
                    Descripcion = definicion.Descripcion,
                    Activo = true
                });
        }

        await dbContext.SaveChangesAsync();
    }


    private static async Task AsignarPermisosARolesAsync(AppDbContext dbContext, RoleManager<ApplicationRole> roleManager)
    {
        /*
         * Traemos todos los permisos una sola vez.
         *
         * ToDictionaryAsync nos permite localizar
         * rápidamente un permiso por su Código.
         */
        Dictionary<string, Permiso> permisos =
            await dbContext.Permisos
                .Where(p => p.Activo)
                .ToDictionaryAsync(
                    p => p.Codigo,
                    p => p);


        foreach (var configuracionRol in MatrizPermisosRoles.PorRol)
        {
            string nombreRol = configuracionRol.Key;


            /*
             * Los roles deberían existir ya
             * porque IdentitySeeder se ejecuta primero.
             */
            ApplicationRole? rol = await roleManager.FindByNameAsync(nombreRol);


            if (rol is null)
            {
                throw new InvalidOperationException(
                    $"No existe el rol '{nombreRol}'. " +
                    "Debe ejecutarse primero IdentitySeeder.");
            }


            IEnumerable<string> codigosPermisos;


            if (nombreRol ==
                RolesSistema.Administrador)
            {
                /*
                 * Administrador recibe todos
                 * los permisos activos.
                 */
                codigosPermisos =
                    permisos.Keys;
            }
            else
            {
                codigosPermisos =
                    configuracionRol.Value;
            }


            /*
             * Recuperamos las relaciones existentes
             * de este rol.
             */
            HashSet<Guid> permisosYaAsignados =
                await dbContext.RolPermisos
                    .Where(rp =>
                        rp.RolId == rol.Id)
                    .Select(rp =>
                        rp.PermisoId)
                    .ToHashSetAsync();


            foreach (
                string codigoPermiso
                in codigosPermisos)
            {
                if (!permisos.TryGetValue(
                    codigoPermiso,
                    out Permiso? permiso))
                {
                    throw new InvalidOperationException(
                        $"No existe el permiso " +
                        $"'{codigoPermiso}'.");
                }


                /*
                 * Idempotencia:
                 *
                 * Si ya existe la relación,
                 * no volvemos a insertarla.
                 */
                if (permisosYaAsignados.Contains(
                    permiso.Id))
                {
                    continue;
                }


                dbContext.RolPermisos.Add(
                    new RolPermiso
                    {
                        RolId = rol.Id,
                        PermisoId = permiso.Id
                    });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}