using e_violenciagen.Data;
using Microsoft.EntityFrameworkCore;

namespace e_violenciagen.Models;

public static class CatalogoSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Si ya existen tipos institucionales,
        // consideramos realizada esta parte del seed.
        if (!await dbContext.TiposInstitucion.AnyAsync(cancellationToken))
        {
            await CrearTiposInstitucionAsync(dbContext, cancellationToken);
        }

        if (!await dbContext.TiposDocumentoIdentidad.AnyAsync(cancellationToken))
        {
            await CrearTiposDocumentoIdentidadAsync(dbContext, cancellationToken);
        }
    }

    private static async Task CrearTiposInstitucionAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var tipos = new[]
        {
            new TipoInstitucion
            {
                Codigo = "MINISTERIO",
                Nombre = "Ministerio",
                Descripcion ="Institución gubernamental encargada de asuntos de estado"
            },

            new TipoInstitucion
            {
                Codigo = "SEGURIDAD",
                Nombre = "Fuerzas y cuerpos de seguridad",
                Descripcion ="Instucion del estado encargada de Seguridad"
            },

            new TipoInstitucion
            {
                Codigo = "FISCALIA",
                Nombre = "Fiscalía",
                Descripcion ="Instucion del estado encargada de Fiscalizar"
            },

            new TipoInstitucion
            {
                Codigo = "JUDICIAL",
                Nombre = "Órgano judicial",
                Descripcion ="Instucion del estado encargada de Justicia"
            },

            new TipoInstitucion
            {
                Codigo = "SANIDAD",
                Nombre = "Institución sanitaria",
                Descripcion ="Instucion del estado encargada de Sanidad"
            }
        };

        await dbContext.TiposInstitucion.AddRangeAsync(
            tipos,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task CrearTiposDocumentoIdentidadAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var tipos = new[]
        {
        new TipoDocumentoIdentidad
        {
            Codigo = "DIP",
            Nombre = "Documento de Identidad Personal"
        },

        new TipoDocumentoIdentidad
        {
            Codigo = "PASAPORTE",
            Nombre = "Pasaporte"
        },

        new TipoDocumentoIdentidad
        {
            Codigo = "OTRO",
            Nombre = "Otro documento"
        }
    };

        await dbContext.TiposDocumentoIdentidad.AddRangeAsync(
            tipos,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
