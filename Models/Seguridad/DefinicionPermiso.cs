namespace e_violenciagen.Models;
/// <summary>
/// Define los datos necesarios para crear
/// un permiso inicial.
///
/// No es una entidad EF Core.
/// Solo se utiliza durante el seeding.
/// </summary>
public sealed record DefinicionPermiso(
    string Codigo,
    string Nombre,
    string Modulo,
    string? Descripcion = null);