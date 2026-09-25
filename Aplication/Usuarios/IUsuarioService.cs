using e_violenciagen.ViewModels;
using e_violenciagen.ViewModels.Seguridad.Usuarios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace e_violenciagen.Aplication;

public interface IUsuarioService
{
    Task<List<UsuarioListItemViewModel>> GetAllAsync(
        CancellationToken cancellationToken = default);


    Task<UsuarioDetailViewModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);


    Task<UsuarioCreateViewModel> PrepararCreateAsync(
        CancellationToken cancellationToken = default);


    Task CrearAsync(
        UsuarioCreateViewModel model,
        CancellationToken cancellationToken = default);


    Task<UsuarioEditViewModel?> PrepararEditAsync(
        Guid id,
        CancellationToken cancellationToken = default);


    Task ActualizarAsync(
        UsuarioEditViewModel model,
        CancellationToken cancellationToken = default);


    Task CambiarEstadoAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SelectListItem>>
    GetUnidadesPorInstitucionAsync(
        Guid institucionId,
        CancellationToken cancellationToken = default);

    /*Task<UsuarioEditViewModel?> PrepararEditAsync(
    Guid id,
    CancellationToken cancellationToken = default);*/


    Task ActualizarAsync(
        UsuarioEditViewModel model,
        Guid usuarioActualId,
        CancellationToken cancellationToken = default);

    Task<UsuarioResetPasswordViewModel?>PrepararResetPasswordAsync(Guid id);

    Task RestablecerPasswordAsync(UsuarioResetPasswordViewModel model);
    Task DesbloquearAsync(Guid id);

    Task<object> GetDataTableAsync(
    int draw,
    int start,
    int length,
    string? search,
    CancellationToken cancellationToken = default);
}