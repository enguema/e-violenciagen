using e_violenciagen.Data;
using e_violenciagen.Models;
using e_violenciagen.ViewModels;
using e_violenciagen.ViewModels.Seguridad.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace e_violenciagen.Aplication;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;


    public UsuarioService(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task ActualizarAsync(UsuarioEditViewModel model, CancellationToken cancellationToken = default)
    {
        ApplicationUser? usuario =
        await _userManager.FindByIdAsync(model.Id.ToString());

        if (usuario is null)
        {
            throw new KeyNotFoundException("El usuario no existe.");
        }


        // =====================================================
        // VALIDAR INSTITUCIÓN
        // =====================================================

        if (!model.InstitucionId.HasValue)
        {
            throw new InvalidOperationException("Debe seleccionar una institución.");
        }

        bool institucionValida =
            await _dbContext.Instituciones
                .AnyAsync(
                    i =>
                        i.Id == model.InstitucionId.Value
                        && i.Activo,
                    cancellationToken);

        if (!institucionValida)
        {
            throw new InvalidOperationException("La institución seleccionada no existe o está inactiva.");
        }


        // =====================================================
        // VALIDAR UNIDAD
        // =====================================================

        if (model.UnidadOrganizativaId.HasValue)
        {
            bool unidadValida =
                await _dbContext.UnidadesOrganizativas
                    .AnyAsync(
                        u =>
                            u.Id == model.UnidadOrganizativaId.Value
                            && u.InstitucionId ==
                               model.InstitucionId.Value,
                        cancellationToken);

            if (!unidadValida)
            {
                throw new InvalidOperationException(
                    "La unidad organizativa seleccionada "
                    + "no pertenece a la institución indicada.");
            }
        }


        // =====================================================
        // VALIDAR USERNAME DUPLICADO
        // =====================================================

        ApplicationUser? usuarioMismoNombre =
            await _userManager.FindByNameAsync(model.NombreUsuario);

        if (usuarioMismoNombre is not null
            && usuarioMismoNombre.Id != usuario.Id)
        {
            throw new InvalidOperationException("Ya existe otro usuario con ese nombre.");
        }


        // =====================================================
        // ACTUALIZAR DATOS
        // =====================================================

        usuario.Nombre =
            model.Nombre.Trim();

        usuario.Apellidos =
            model.Apellidos.Trim();

        usuario.InstitucionId =
            model.InstitucionId.Value;

        usuario.UnidadOrganizativaId =
            model.UnidadOrganizativaId;


        IdentityResult nombreResultado =
            await _userManager.SetUserNameAsync(
                usuario,
                model.NombreUsuario.Trim());

        ValidarIdentityResult(
            nombreResultado,
            "actualizar el nombre de usuario");


        IdentityResult emailResultado =
            await _userManager.SetEmailAsync(
                usuario,
                string.IsNullOrWhiteSpace(model.Email)
                    ? null!
                    : model.Email.Trim());

        ValidarIdentityResult(
            emailResultado,
            "actualizar el correo electrónico");


        IdentityResult resultadoUsuario =
            await _userManager.UpdateAsync(usuario);

        ValidarIdentityResult(
            resultadoUsuario,
            "actualizar el usuario");


        // =====================================================
        // ACTUALIZAR ROLES
        // =====================================================

        await ActualizarRolesAsync(
            usuario,
            model.RolesSeleccionados,
            //usuarioActualId,
            //usuario.Id,
            model.Id,
            cancellationToken);
    }

    public async Task CambiarEstadoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationUser? usuario = await _userManager.FindByIdAsync(id.ToString());

        if (usuario is null)
        {
            throw new KeyNotFoundException(
                "El usuario no existe.");
        }

        if (usuario.Id == id && usuario.Activo)
        {
            throw new InvalidOperationException(
                "No puede desactivar su propia cuenta.");
        }

        if (usuario.Activo)
        {
            bool esAdministrador =
                await _userManager.IsInRoleAsync(
                    usuario,
                    RolesSistema.Administrador);

            if (esAdministrador)
            {
                await ValidarQueNoSeaUltimoAdministradorAsync(
                    usuario.Id,
                    cancellationToken);
            }
        }


        usuario.Activo = !usuario.Activo;


        /*
         * Si lo estamos desactivando, invalidamos
         * también las sesiones existentes.
         *
         * El SecurityStamp permite que Identity
         * invalide cookies cuando corresponda.
         */
        if (!usuario.Activo)
        {
            await _userManager.UpdateSecurityStampAsync(
                usuario);
        }


        IdentityResult resultado =
            await _userManager.UpdateAsync(usuario);

        ValidarIdentityResult(
            resultado,
            "actualizar el estado del usuario");
    }

    public async Task CrearAsync(UsuarioCreateViewModel model, CancellationToken cancellationToken = default)
    {
        // =====================================================
        // 1. VALIDAR INSTITUCIÓN
        // =====================================================

        if (!model.InstitucionId.HasValue)
        {
            throw new InvalidOperationException(
                "Debe seleccionar una institución.");
        }


        bool institucionExiste =
            await _dbContext.Instituciones
                .AnyAsync(
                    i =>
                        i.Id == model.InstitucionId.Value
                        && i.Activo,
                    cancellationToken);

        if (!institucionExiste)
        {
            throw new InvalidOperationException(
                "La institución seleccionada no existe o está inactiva.");
        }


        // =====================================================
        // 2. VALIDAR UNIDAD
        // =====================================================

        if (model.UnidadOrganizativaId.HasValue)
        {
            bool unidadValida =
                await _dbContext.UnidadesOrganizativas
                    .AnyAsync(
                        u =>
                            u.Id == model.UnidadOrganizativaId.Value
                            && u.InstitucionId
                                == model.InstitucionId.Value,
                        cancellationToken);

            if (!unidadValida)
            {
                throw new InvalidOperationException(
                    "La unidad organizativa seleccionada "
                    + "no pertenece a la institución indicada.");
            }
        }


        // =====================================================
        // 3. COMPROBAR USUARIO DUPLICADO
        // =====================================================

        ApplicationUser? existente =
            await _userManager.FindByNameAsync(
                model.NombreUsuario);

        if (existente is not null)
        {
            throw new InvalidOperationException(
                "Ya existe un usuario con ese nombre de usuario.");
        }


        // =====================================================
        // 4. CREAR USUARIO
        // =====================================================

        var usuario = new ApplicationUser
        {
            Nombre = model.Nombre.Trim(),

            Apellidos = model.Apellidos.Trim(),

            UserName = model.NombreUsuario.Trim(),

            Email = string.IsNullOrWhiteSpace(model.Email)
                ? null
                : model.Email.Trim(),

            InstitucionId =
                model.InstitucionId.Value,

            UnidadOrganizativaId =
                model.UnidadOrganizativaId,

            Activo = true,

            FechaCreacion = DateTime.UtcNow,

            LockoutEnabled = true
        };


        IdentityResult resultado =
            await _userManager.CreateAsync(
                usuario,
                model.Password);


        ValidarIdentityResult(
            resultado,
            "crear el usuario");


        // =====================================================
        // 5. ASIGNAR ROLES
        // =====================================================

        if (model.RolesSeleccionados.Count == 0)
        {
            return;
        }


        List<string> roles =
            await _dbContext.Roles
                .AsNoTracking()
                .Where(r =>
                    model.RolesSeleccionados.Contains(r.Id)
                    && r.Activo)
                .Select(r => r.Name!)
                .ToListAsync(cancellationToken);


        if (roles.Count > 0)
        {
            IdentityResult resultadoRoles =
                await _userManager.AddToRolesAsync(
                    usuario,
                    roles);

            ValidarIdentityResult(
                resultadoRoles,
                "asignar los roles al usuario");
        }
    }

    public async Task<List<UsuarioListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Institucion)
            .Include(u => u.UnidadOrganizativa)
            .OrderBy(u => u.Apellidos)
            .ThenBy(u => u.Nombre)
            .ToListAsync(cancellationToken);


        var resultado =
            new List<UsuarioListItemViewModel>();


        foreach (ApplicationUser usuario in usuarios)
        {
            IList<string> roles =
                await _userManager.GetRolesAsync(usuario);

            resultado.Add(
                new UsuarioListItemViewModel
                {
                    Id = usuario.Id,

                    NombreUsuario =
                        usuario.UserName ?? string.Empty,

                    NombreCompleto =
                        $"{usuario.Nombre} {usuario.Apellidos}",

                    Email = usuario.Email,

                    Institucion =
                        usuario.Institucion.Nombre,

                    UnidadOrganizativa =
                        usuario.UnidadOrganizativa?.Nombre,

                    Activo = usuario.Activo,

                    Bloqueado =
                        usuario.LockoutEnd.HasValue
                        && usuario.LockoutEnd.Value
                            > DateTimeOffset.UtcNow,

                    FechaCreacion =
                        usuario.FechaCreacion,

                    UltimoAcceso =
                        usuario.UltimoAcceso,

                    Roles = roles.ToList()
                });
        }


        return resultado;
    }

    public async Task<UsuarioDetailViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationUser? usuario =
            await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Institucion)
                .Include(u => u.UnidadOrganizativa)
                .FirstOrDefaultAsync(
                    u => u.Id == id,
                    cancellationToken);


        if (usuario is null)
        {
            return null;
        }


        IList<string> roles =
            await _userManager.GetRolesAsync(
                usuario);


        return new UsuarioDetailViewModel
        {
            Id = usuario.Id,

            NombreUsuario =
                usuario.UserName ?? string.Empty,

            NombreCompleto =
                $"{usuario.Nombre} {usuario.Apellidos}",

            Email = usuario.Email,

            Institucion =
                usuario.Institucion.Nombre,

            UnidadOrganizativa =
                usuario.UnidadOrganizativa?.Nombre,

            Activo =
                usuario.Activo,

            FechaCreacion =
                usuario.FechaCreacion,

            UltimoAcceso =
                usuario.UltimoAcceso,

            LockoutEnd =
                usuario.LockoutEnd,

            Roles =
                roles.ToList()
        };
    }

    public async Task<UsuarioCreateViewModel> PrepararCreateAsync(CancellationToken cancellationToken = default)
    {
        var model = new UsuarioCreateViewModel();

        await CargarCombosAsync(
            model,
            cancellationToken);

        return model;
    }

    public async Task<UsuarioEditViewModel?> PrepararEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationUser? usuario =
       await _dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(
               u => u.Id == id,
               cancellationToken);

        if (usuario is null)
        {
            return null;
        }


        IList<string> rolesActuales =
            await _userManager.GetRolesAsync(usuario);


        List<Guid> rolesSeleccionados =
            await _dbContext.Roles
                .AsNoTracking()
                .Where(r =>
                    r.Name != null &&
                    rolesActuales.Contains(r.Name))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);


        var model = new UsuarioEditViewModel
        {
            Id = usuario.Id,

            Nombre = usuario.Nombre,

            Apellidos = usuario.Apellidos,

            NombreUsuario =
                usuario.UserName ?? string.Empty,

            Email = usuario.Email,

            InstitucionId =
                usuario.InstitucionId,

            UnidadOrganizativaId =
                usuario.UnidadOrganizativaId,

            RolesSeleccionados =
                rolesSeleccionados
        };

        await CargarCombosEditAsync(model, cancellationToken);

        return model;
    }

    public async Task<IReadOnlyList<SelectListItem>>
    GetUnidadesPorInstitucionAsync(Guid institucionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .Where(u =>
                u.InstitucionId == institucionId)
            .OrderBy(u => u.Nombre)
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Nombre
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UsuarioResetPasswordViewModel?> PrepararResetPasswordAsync(Guid id)
    {
        ApplicationUser? usuario =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (usuario is null)
        {
            return null;
        }


        return new UsuarioResetPasswordViewModel
        {
            UsuarioId = usuario.Id,
            NombreUsuario =
                usuario.UserName ?? string.Empty
        };
    }

    public async Task RestablecerPasswordAsync(UsuarioResetPasswordViewModel model)
    {
        ApplicationUser? usuario =
            await _userManager.FindByIdAsync(
                model.UsuarioId.ToString());

        if (usuario is null)
        {
            throw new KeyNotFoundException(
                "El usuario no existe.");
        }


        string token =
            await _userManager
                .GeneratePasswordResetTokenAsync(
                    usuario);


        IdentityResult resultado =
            await _userManager.ResetPasswordAsync(
                usuario,
                token,
                model.NuevaPassword);


        ValidarIdentityResult(
            resultado,
            "restablecer la contraseña");


        /*
         * Invalidamos las sesiones existentes.
         *
         * Así evitamos que una cookie antigua continúe
         * siendo válida después de un reset administrativo.
         */
        IdentityResult stampResultado =
            await _userManager.UpdateSecurityStampAsync(
                usuario);

        ValidarIdentityResult(
            stampResultado,
            "invalidar las sesiones anteriores");
    }

    public async Task DesbloquearAsync(Guid id)
    {
        ApplicationUser? usuario =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (usuario is null)
        {
            throw new KeyNotFoundException(
                "El usuario no existe.");
        }


        IdentityResult desbloqueo =
            await _userManager.SetLockoutEndDateAsync(
                usuario,
                null);

        ValidarIdentityResult(
            desbloqueo,
            "desbloquear el usuario");


        IdentityResult contador =
            await _userManager.ResetAccessFailedCountAsync(
                usuario);

        ValidarIdentityResult(
            contador,
            "reiniciar los intentos fallidos");
    }

    public async Task<object> GetDataTableAsync(int draw, int start, int length, string? search,
    CancellationToken cancellationToken = default)
    {
        /*
         * Consulta base.
         *
         * AsNoTracking porque solo estamos leyendo.
         */
        var query =
            _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Institucion)
                .Include(u => u.UnidadOrganizativa)
                .AsQueryable();


        int recordsTotal =
            await query.CountAsync(cancellationToken);


        // =========================================================
        // FILTRO GLOBAL DATATABLE
        // =========================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();

            query = query.Where(u =>
                (u.UserName != null &&
                 u.UserName.ToLower().Contains(search))
                ||
                u.Nombre.ToLower().Contains(search)
                ||
                u.Apellidos.ToLower().Contains(search)
                ||
                u.Institucion.Nombre.ToLower().Contains(search));
        }


        int recordsFiltered =
            await query.CountAsync(cancellationToken);


        // =========================================================
        // PAGINACIÓN
        // =========================================================

        var usuarios =
            await query
                .OrderBy(u => u.Apellidos)
                .ThenBy(u => u.Nombre)
                .Skip(start)
                .Take(length)
                .ToListAsync(cancellationToken);


        var data =
            new List<UsuarioDataTableItemViewModel>();


        foreach (var usuario in usuarios)
        {
            IList<string> roles =
                await _userManager.GetRolesAsync(usuario);


            data.Add(
                new UsuarioDataTableItemViewModel
                {
                    Id = usuario.Id,

                    NombreUsuario =
                        usuario.UserName ?? string.Empty,

                    NombreCompleto =
                        $"{usuario.Nombre} {usuario.Apellidos}",

                    Institucion =
                        usuario.Institucion.Nombre,

                    UnidadOrganizativa =
                        usuario.UnidadOrganizativa?.Nombre
                        ?? string.Empty,

                    Roles =
                        string.Join(", ", roles),

                    Activo =
                        usuario.Activo,

                    Bloqueado =
                        usuario.LockoutEnd.HasValue
                        &&
                        usuario.LockoutEnd.Value
                            > DateTimeOffset.UtcNow,

                    UltimoAcceso =
                        usuario.UltimoAcceso
                });
        }


        return new
        {
            draw,
            recordsTotal,
            recordsFiltered,
            data
        };
    }

    /*====== Helpers ======*/
    private async Task CargarCombosAsync(UsuarioCreateViewModel model, CancellationToken cancellationToken)
    {
        model.Instituciones =
            await _dbContext.Instituciones
                .AsNoTracking()
                .Where(i => i.Activo)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Nombre
                })
                .ToListAsync(cancellationToken);


        model.Roles =
            await _dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Activo)
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name!
                })
                .ToListAsync(cancellationToken);
    }

    private static void ValidarIdentityResult(IdentityResult resultado, string operacion)
    {
        if (resultado.Succeeded)
        {
            return;
        }

        string errores = string.Join(
            "; ",
            resultado.Errors.Select(
                e => e.Description));


        throw new InvalidOperationException(
            $"No se pudo {operacion}: {errores}");
    }

    private async Task CargarCombosEditAsync(UsuarioEditViewModel model, CancellationToken cancellationToken)
    {
        model.Instituciones =
            await _dbContext.Instituciones
                .AsNoTracking()
                .Where(i => i.Activo)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Nombre
                })
                .ToListAsync(cancellationToken);


        model.Roles =
            await _dbContext.Roles
                .AsNoTracking()
                .Where(r => r.Activo)
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name!
                })
                .ToListAsync(cancellationToken);


        if (model.InstitucionId.HasValue)
        {
            model.UnidadesOrganizativas =
                await _dbContext.UnidadesOrganizativas
                    .AsNoTracking()
                    .Where(u =>
                        u.InstitucionId ==
                        model.InstitucionId.Value)
                    .OrderBy(u => u.Nombre)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Nombre
                    })
                    .ToListAsync(cancellationToken);
        }
    }

    private async Task ActualizarRolesAsync(
    ApplicationUser usuario,
    IReadOnlyCollection<Guid> rolesSeleccionados,
    Guid usuarioActualId,
    CancellationToken cancellationToken)
    {
        IList<string> rolesActuales =
            await _userManager.GetRolesAsync(usuario);


        List<string> rolesNuevos =
            await _dbContext.Roles
                .AsNoTracking()
                .Where(r =>
                    rolesSeleccionados.Contains(r.Id)
                    && r.Activo
                    && r.Name != null)
                .Select(r => r.Name!)
                .ToListAsync(cancellationToken);


        // =====================================================
        // PROTECCIÓN DEL ADMINISTRADOR
        // =====================================================

        bool eraAdministrador =
            rolesActuales.Contains(
                RolesSistema.Administrador);

        bool seguiraAdministrador =
            rolesNuevos.Contains(
                RolesSistema.Administrador);


        if (eraAdministrador && !seguiraAdministrador)
        {
            /*
             * Un usuario no debe poder quitarse
             * a sí mismo su propio rol Administrador.
             */
            if (usuario.Id == usuarioActualId)
            {
                throw new InvalidOperationException(
                    "No puede quitarse a sí mismo "
                    + "el rol Administrador.");
            }


            /*
             * Además evitamos eliminar el rol al último
             * administrador operativo del sistema.
             */
            await ValidarQueNoSeaUltimoAdministradorAsync(
                usuario.Id,
                cancellationToken);
        }


        // =====================================================
        // CALCULAR DIFERENCIAS
        // =====================================================

        string[] rolesAEliminar =
            rolesActuales
                .Except(
                    rolesNuevos,
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();


        string[] rolesAAgregar =
            rolesNuevos
                .Except(
                    rolesActuales,
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();


        if (rolesAEliminar.Length > 0)
        {
            IdentityResult eliminarResultado =
                await _userManager.RemoveFromRolesAsync(
                    usuario,
                    rolesAEliminar);

            ValidarIdentityResult(
                eliminarResultado,
                "quitar roles del usuario");
        }


        if (rolesAAgregar.Length > 0)
        {
            IdentityResult agregarResultado =
                await _userManager.AddToRolesAsync(
                    usuario,
                    rolesAAgregar);

            ValidarIdentityResult(
                agregarResultado,
                "asignar roles al usuario");
        }
    }

    private async Task ValidarQueNoSeaUltimoAdministradorAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        ApplicationRole? rolAdministrador =
            await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.Name ==
                        RolesSistema.Administrador,
                    cancellationToken);

        if (rolAdministrador is null)
        {
            throw new InvalidOperationException(
                "No se encontró el rol Administrador.");
        }


        int administradoresActivos =
            await (
                from userRole in _dbContext.UserRoles
                join usuario in _dbContext.Users
                    on userRole.UserId equals usuario.Id

                where
                    userRole.RoleId == rolAdministrador.Id
                    && usuario.Activo

                select usuario.Id
            )
            .Distinct()
            .CountAsync(cancellationToken);


        if (administradoresActivos <= 1)
        {
            throw new InvalidOperationException(
                "No puede quitarse el rol Administrador "
                + "al último administrador activo del sistema.");
        }
    }

    public Task ActualizarAsync(UsuarioEditViewModel model, Guid usuarioActualId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}