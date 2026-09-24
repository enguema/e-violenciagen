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

    public Task ActualizarAsync(UsuarioEditViewModel model, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task CambiarEstadoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationUser? usuario =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (usuario is null)
        {
            throw new KeyNotFoundException(
                "El usuario no existe.");
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

    public Task<UsuarioDetailViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UsuarioCreateViewModel> PrepararCreateAsync(CancellationToken cancellationToken = default)
    {
        var model = new UsuarioCreateViewModel();

        await CargarCombosAsync(
            model,
            cancellationToken);

        return model;
    }

    public Task<UsuarioEditViewModel?> PrepararEditAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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


}