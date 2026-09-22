using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using e_violenciagen.Models;
using Microsoft.Extensions.Options;

namespace e_violenciagen.Data;
/// <summary>
/// Inicializa los datos mínimos requeridos por Identity.
///
/// Su responsabilidad es únicamente:
///
/// - crear roles base;
/// - crear el administrador inicial.
///
/// No contiene lógica de login ni autorización.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        /*
         * Creamos un scope porque UserManager,
         * RoleManager y AppDbContext son servicios scoped.
         */
        using IServiceScope scope = services.CreateScope();


        RoleManager<ApplicationRole> roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<ApplicationRole>>();


        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();


        AppDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();


        AdministradorInicialOptions adminOptions =
            scope.ServiceProvider
                .GetRequiredService<
                    IOptions<AdministradorInicialOptions>>()
                .Value;


        // =====================================================
        // 1. CREAR ROLES BASE
        // =====================================================

        await CrearRolesAsync(roleManager);


        // =====================================================
        // 2. CREAR ADMINISTRADOR INICIAL
        // =====================================================

        await CrearAdministradorAsync(
            userManager,
            dbContext,
            adminOptions);
        
    }


    private static async Task CrearRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (string nombreRol in RolesSistema.Todos)
        {
            /*
             * RoleExistsAsync consulta usando los mecanismos
             * propios de ASP.NET Core Identity.
             */
            bool existe = await roleManager
                .RoleExistsAsync(nombreRol);

            if (existe)
            {
                continue;
            }


            var rol = new ApplicationRole
            {
                Name = nombreRol,

                Activo = true,

                /*
                 * Estos roles forman parte de la configuración
                 * estructural inicial del sistema.
                 */
                EsSistema = true,

                Descripcion = ObtenerDescripcionRol(nombreRol)
            };


            IdentityResult resultado =
                await roleManager.CreateAsync(rol);


            if (!resultado.Succeeded)
            {
                string errores = string.Join(
                    "; ",
                    resultado.Errors.Select(
                        e => $"{e.Code}: {e.Description}"));

                throw new InvalidOperationException(
                    $"No se pudo crear el rol '{nombreRol}'. "
                    + errores);
            }
        }
    }

    private static async Task CrearAdministradorAsync(UserManager<ApplicationUser> userManager, AppDbContext dbContext, AdministradorInicialOptions options)
    {
        // =====================================================
        // VALIDAR CONFIGURACIÓN
        // =====================================================

        if (string.IsNullOrWhiteSpace(
            options.NombreUsuario))
        {
            throw new InvalidOperationException(
                "No se ha configurado " +
                "'AdministradorInicial:NombreUsuario'.");
        }


        if (string.IsNullOrWhiteSpace(
            options.Password))
        {
            throw new InvalidOperationException(
                "No se ha configurado " +
                "'AdministradorInicial:Password'.");
        }


        if (string.IsNullOrWhiteSpace(
            options.CodigoInstitucion))
        {
            throw new InvalidOperationException(
                "No se ha configurado " +
                "'AdministradorInicial:CodigoInstitucion'.");
        }


        // =====================================================
        // COMPROBAR SI EL USUARIO YA EXISTE
        // =====================================================

        ApplicationUser? usuarioExistente =
            await userManager.FindByNameAsync(
                options.NombreUsuario);

        if (usuarioExistente is not null)
        {
            /*
             * Aunque el usuario ya exista, comprobamos
             * que siga teniendo el rol Administrador.
             */
            bool tieneRol =
                await userManager.IsInRoleAsync(
                    usuarioExistente,
                    RolesSistema.Administrador);

            if (!tieneRol)
            {
                IdentityResult resultadoRol =
                    await userManager.AddToRoleAsync(
                        usuarioExistente,
                        RolesSistema.Administrador);

                ValidarResultado(
                    resultadoRol,
                    "asignar el rol Administrador");
            }

            return;
        }


        // =====================================================
        // OBTENER INSTITUCIÓN
        // =====================================================

        /*
         * Buscamos por código y no por Guid.
         *
         * El código es estable mientras que el Guid
         * puede cambiar si recreamos la base de datos.
         */
        var institucion =
            await dbContext.Instituciones
                .FirstOrDefaultAsync(i =>
                    i.Codigo ==
                    options.CodigoInstitucion);

        if (institucion is null)
        {
            throw new InvalidOperationException(
                $"No existe una institución con código " +
                $"'{options.CodigoInstitucion}'. " +
                "Compruebe que el Seeder institucional " +
                "se haya ejecutado.");
        }


        // =====================================================
        // CREAR ADMINISTRADOR
        // =====================================================

        var administrador = new ApplicationUser
        {
            UserName = options.NombreUsuario,

            Email = string.IsNullOrWhiteSpace(
                options.Email)
                ? null
                : options.Email,

            Nombre = options.Nombre,

            Apellidos = options.Apellidos,


            /*
             * Utilizamos el Id real recuperado
             * desde PostgreSQL.
             */
            InstitucionId = institucion.Id,


            /*
             * Por ahora el administrador pertenece
             * directamente a la institución.
             */
            UnidadOrganizativaId = null,

            Activo = true,

            FechaCreacion = DateTime.UtcNow,

            LockoutEnabled = true
        };


        IdentityResult resultadoUsuario =
            await userManager.CreateAsync(
                administrador,
                options.Password);


        ValidarResultado(
            resultadoUsuario,
            "crear el administrador inicial");


        // =====================================================
        // ASIGNAR ROL ADMINISTRADOR
        // =====================================================

        IdentityResult resultadoRolAdministrador =
            await userManager.AddToRoleAsync(
                administrador,
                RolesSistema.Administrador);


        ValidarResultado(
            resultadoRolAdministrador,
            "asignar el rol Administrador");
    }
    
    private static async Task CrearAdministradorAsyncBORRAR(
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        AdministradorInicialOptions options)
    {
        /*
         * Primero comprobamos si el administrador
         * ya existe.
         *
         * Esto hace que el proceso sea idempotente.
         */
        ApplicationUser? usuarioExistente =
            await userManager.FindByNameAsync(
                options.NombreUsuario);

        if (usuarioExistente is not null)
        {
            /*
             * Aunque el usuario ya exista, comprobamos
             * que conserve el rol Administrador.
             */
            bool tieneRol =
                await userManager.IsInRoleAsync(
                    usuarioExistente,
                    RolesSistema.Administrador);

            if (!tieneRol)
            {
                IdentityResult asignacion =
                    await userManager.AddToRoleAsync(
                        usuarioExistente,
                        RolesSistema.Administrador);

                ValidarResultado(
                    asignacion,
                    "asignar el rol Administrador");
            }

            return;
        }


        // =====================================================
        // VALIDACIONES DE CONFIGURACIÓN
        // =====================================================

        if (string.IsNullOrWhiteSpace(options.NombreUsuario))
        {
            throw new InvalidOperationException(
                "No se ha configurado "
                + "'AdministradorInicial:NombreUsuario'.");
        }


        if (string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException(
                "No se ha configurado "
                + "'AdministradorInicial:Password'.");
        }


        /*
         * El administrador debe apuntar a una institución
         * realmente existente.
         */
        /*bool institucionExiste =
            await dbContext.Instituciones
                .AnyAsync(i =>
                    i.Id == options.InstitucionId);*/
        var institucion = await dbContext.Instituciones
            .FirstOrDefaultAsync(i =>
                i.Codigo == options.CodigoInstitucion);

        /*if (!institucionExiste)
        {
            throw new InvalidOperationException(
                $"La institución '{options.InstitucionId}' "
                + "configurada para el administrador "
                + "no existe.");
        }*/
        if (institucion is null)
        {
            throw new InvalidOperationException(
                $"No existe una institución con código " +
                $"'{options.CodigoInstitucion}'.");
        }



        // =====================================================
        // CREACIÓN DEL USUARIO
        // =====================================================

        var administrador = new ApplicationUser
        {
            UserName = options.NombreUsuario,

            Email = string.IsNullOrWhiteSpace(options.Email)
                ? null
                : options.Email,

            Nombre = options.Nombre,

            Apellidos = options.Apellidos,

            //InstitucionId = options.InstitucionId,
            InstitucionId = institucion.Id,

            /*
             * El administrador inicial no necesita
             * obligatoriamente una unidad.
             */
            UnidadOrganizativaId = null,

            Activo = true,

            FechaCreacion = DateTime.UtcNow,

            /*
             * Permitimos que Identity pueda utilizar
             * posteriormente su mecanismo de bloqueo.
             */
            LockoutEnabled = true
        };


        /*
         * MUY IMPORTANTE:
         *
         * NO asignamos PasswordHash manualmente.
         *
         * UserManager.CreateAsync se encarga de:
         *
         * - validar la contraseña;
         * - generar el hash;
         * - establecer las propiedades de seguridad;
         * - guardar el usuario.
         */
        IdentityResult resultadoUsuario =
            await userManager.CreateAsync(
                administrador,
                options.Password);


        ValidarResultado(
            resultadoUsuario,
            "crear el administrador inicial");


        // =====================================================
        // ASIGNAR ROL ADMINISTRADOR
        // =====================================================

        IdentityResult resultadoRol =
            await userManager.AddToRoleAsync(
                administrador,
                RolesSistema.Administrador);


        ValidarResultado(
            resultadoRol,
            "asignar el rol Administrador");
    }


    private static void ValidarResultado(
        IdentityResult resultado,
        string operacion)
    {
        if (resultado.Succeeded)
        {
            return;
        }


        string errores = string.Join(
            "; ",
            resultado.Errors.Select(
                e => $"{e.Code}: {e.Description}"));

        throw new InvalidOperationException(
            $"Error al {operacion}: {errores}");
    }


    private static string ObtenerDescripcionRol(
        string nombreRol)
    {
        return nombreRol switch
        {
            RolesSistema.Administrador =>
                "Administración general del sistema.",

            RolesSistema.GestorCasos =>
                "Gestión operativa de casos.",

            RolesSistema.Supervisor =>
                "Supervisión y seguimiento de casos.",

            RolesSistema.Consulta =>
                "Acceso principalmente orientado a consulta.",

            _ => "Rol del sistema."
        };
    }
}