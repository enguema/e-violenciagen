using e_violenciagen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_violenciagen.Seguridad;

/// <summary>
/// Evalúa si el usuario autenticado posee
/// el permiso requerido a través de alguno
/// de sus roles.
///
/// Flujo:
///
/// Usuario
///   -> AspNetUserRoles
///   -> AspNetRoles
///   -> RolPermisos
///   -> Permisos
/// </summary>
public sealed class PermisoAuthorizationHandler
    : AuthorizationHandler<PermisoRequirement>
{
    private readonly AppDbContext _dbContext;


    public PermisoAuthorizationHandler(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermisoRequirement requirement)
    {
        // =====================================================
        // 1. COMPROBAR AUTENTICACIÓN
        // =====================================================

        if (context.User.Identity?.IsAuthenticated != true)
        {
            /*
             * No llamamos a Succeed.
             *
             * Al no satisfacerse el requisito,
             * ASP.NET Core denegará la autorización.
             */
            return;
        }


        // =====================================================
        // 2. OBTENER IDENTIFICADOR DEL USUARIO
        // =====================================================

        /*
         * ASP.NET Core Identity almacena normalmente
         * el Id del usuario en NameIdentifier.
         */
        string? userIdValue =
            context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (!Guid.TryParse(
            userIdValue,
            out Guid userId))
        {
            return;
        }


        // =====================================================
        // 3. CONSULTAR EL PERMISO
        // =====================================================

        /*
         * Realizamos una única consulta.
         *
         * Para que la autorización sea válida:
         *
         * - el usuario debe seguir activo;
         * - debe tener un rol activo;
         * - el rol debe tener el permiso solicitado;
         * - el permiso debe seguir activo.
         *
         * AsNoTracking mejora esta consulta porque
         * únicamente estamos leyendo información.
         */
        bool tienePermiso =
            await (
                from usuario in _dbContext.Users.AsNoTracking()

                join usuarioRol in _dbContext.UserRoles.AsNoTracking()
                    on usuario.Id equals usuarioRol.UserId

                join rol in _dbContext.Roles.AsNoTracking()
                    on usuarioRol.RoleId equals rol.Id

                join rolPermiso in _dbContext.RolPermisos.AsNoTracking()
                    on rol.Id equals rolPermiso.RolId

                join permiso in _dbContext.Permisos.AsNoTracking()
                    on rolPermiso.PermisoId equals permiso.Id

                where
                    usuario.Id == userId
                    && usuario.Activo
                    && rol.Activo
                    && permiso.Activo
                    && permiso.Codigo == requirement.CodigoPermiso

                select permiso.Id

            ).AnyAsync();


        // =====================================================
        // 4. MARCAR EL REQUISITO COMO SATISFECHO
        // =====================================================

        if (tienePermiso)
        {
            context.Succeed(requirement);
        }
    }
}