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

        if (!await dbContext.EstadosCaso.AnyAsync(cancellationToken))
        {
            await CrearEstadosCasoAsync(
                dbContext,
                cancellationToken);
        }

        if (!await dbContext.TiposViolencia.AnyAsync(cancellationToken))
        {
            await CrearTiposViolenciaAsync(dbContext, cancellationToken);
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

    private static async Task CrearEstadosCasoAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        /*
         * Estos son estados iniciales de trabajo.
         *
         * No deben considerarse todavía una definición
         * jurídica definitiva del ciclo de vida de un caso.
         * Podremos modificarlos cuando se validen los
         * procedimientos institucionales.
         */
        var estados = new[]
        {
        new EstadoCaso
        {
            Codigo = "REGISTRADO",
            Nombre = "Registrado",
            Descripcion = "Caso registrado inicialmente en SIGEVIG.",
            Orden = 1
        },

        new EstadoCaso
        {
            Codigo = "EN_SEGUIMIENTO",
            Nombre = "En seguimiento",
            Descripcion = "Caso actualmente atendido o en seguimiento.",
            Orden = 2
        },

        new EstadoCaso
        {
            Codigo = "CERRADO",
            Nombre = "Cerrado",
            Descripcion = "Caso cuyo seguimiento ha finalizado.",
            Orden = 3
        }
    };

        await dbContext.EstadosCaso.AddRangeAsync(
            estados,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task CrearTiposViolenciaAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        /*
         * CATÁLOGO PROVISIONAL.
         *
         * Estos valores permiten desarrollar y probar el sistema.
         * Deben validarse posteriormente contra la normativa y
         * protocolos oficiales aplicables en Guinea Ecuatorial.
         */
        var tipos = new[]
        {
        new TipoViolencia
        {
            Codigo = "FISICA",
            Nombre = "Violencia física"
        },

        new TipoViolencia
        {
            Codigo = "PSICOLOGICA",
            Nombre = "Violencia psicológica"
        },

        new TipoViolencia
        {
            Codigo = "SEXUAL",
            Nombre = "Violencia sexual"
        },

        new TipoViolencia
        {
            Codigo = "ECONOMICA",
            Nombre = "Violencia económica"
        }
    };

        await dbContext.TiposViolencia.AddRangeAsync(
            tipos,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    
}
