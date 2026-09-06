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

        if (!await dbContext.TiposActuacion.AnyAsync(cancellationToken))
        {
            await CrearTiposActuacionAsync(
                dbContext,
                cancellationToken);
        }

        if (!await dbContext.TiposDocumentoExpediente
        .AnyAsync(cancellationToken))
        {
            await CrearTiposDocumentoExpedienteAsync(
                dbContext,
                cancellationToken);
        }
    }

    private static async Task CrearTiposDocumentoExpedienteAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        /*
         * Catálogo inicial de desarrollo.
         *
         * Posteriormente deberá validarse con los procedimientos
         * documentales de las instituciones participantes.
         */
        var tipos = new[]
        {
        new TipoDocumentoExpediente
        {
            Codigo = "DENUNCIA",
            Nombre = "Denuncia"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "INFORME_POLICIAL",
            Nombre = "Informe policial"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "INFORME_MEDICO",
            Nombre = "Informe médico"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "INFORME_PSICOLOGICO",
            Nombre = "Informe psicológico"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "INFORME_SOCIAL",
            Nombre = "Informe social"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "INFORME_FISCAL",
            Nombre = "Documento fiscal"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "RESOLUCION_JUDICIAL",
            Nombre = "Resolución judicial"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "DECLARACION",
            Nombre = "Declaración"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "EVIDENCIA",
            Nombre = "Evidencia documental"
        },

        new TipoDocumentoExpediente
        {
            Codigo = "OTRO",
            Nombre = "Otro documento"
        }
    };

        await dbContext.TiposDocumentoExpediente.AddRangeAsync(
            tipos,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task CrearTiposActuacionAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        /*
         * Catálogo inicial de desarrollo.
         *
         * Deberá revisarse posteriormente con los procedimientos
         * reales de Policía, Sanidad, Fiscalía, Juzgados y
         * Ministerio.
         */
        var tipos = new[]
        {
        new TipoActuacion
        {
            Codigo = "DENUNCIA",
            Nombre = "Recepción de denuncia",
            Descripcion =
                "Registro o recepción formal de una denuncia."
        },

        new TipoActuacion
        {
            Codigo = "ATENCION_MEDICA",
            Nombre = "Atención médica",
            Descripcion =
                "Atención sanitaria relacionada con el caso."
        },

        new TipoActuacion
        {
            Codigo = "EVALUACION_PSICOLOGICA",
            Nombre = "Evaluación psicológica"
        },

        new TipoActuacion
        {
            Codigo = "EVALUACION_SOCIAL",
            Nombre = "Evaluación social"
        },

        new TipoActuacion
        {
            Codigo = "DILIGENCIA_POLICIAL",
            Nombre = "Diligencia policial"
        },

        new TipoActuacion
        {
            Codigo = "REMISION",
            Nombre = "Remisión de expediente"
        },

        new TipoActuacion
        {
            Codigo = "ACTUACION_FISCAL",
            Nombre = "Actuación fiscal"
        },

        new TipoActuacion
        {
            Codigo = "ACTUACION_JUDICIAL",
            Nombre = "Actuación judicial"
        },

        new TipoActuacion
        {
            Codigo = "SEGUIMIENTO_SOCIAL",
            Nombre = "Seguimiento social"
        },

        new TipoActuacion
        {
            Codigo = "OTRA",
            Nombre = "Otra actuación"
        }
    };

        await dbContext.TiposActuacion.AddRangeAsync(
            tipos,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
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
