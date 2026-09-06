using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;
/// <summary>
/// Representa todos los datos necesarios para registrar
/// inicialmente un expediente.
/// </summary>
public class CasoCreateViewModel : IValidatableObject
{
    // =========================================================
    // CASO
    // =========================================================

    [Display(Name = "Fecha del hecho")]
    [DataType(DataType.Date)]
    public DateOnly? FechaHecho { get; set; }


    [Display(Name = "Provincia")]
    public Guid? ProvinciaId { get; set; }


    [Display(Name = "Distrito")]
    public Guid? DistritoId { get; set; }


    [Display(Name = "Barrio")]
    public Guid? BarrioHechoId { get; set; }


    [Display(Name = "Referencia del lugar")]
    [MaxLength(500)]
    public string? LugarDescripcion { get; set; }


    [Display(Name = "Resumen del caso")]
    [MaxLength(500)]
    public string? Resumen { get; set; }


    [Display(Name = "Relato inicial")]
    [MaxLength(5000)]
    public string? RelatoInicial { get; set; }


    /// <summary>
    /// Un caso puede presentar varios tipos de violencia.
    /// </summary>
    public List<Guid> TiposViolenciaIds { get; set; } = [];


    // =========================================================
    // VÍCTIMA PRINCIPAL
    // =========================================================

    public PersonaRegistroViewModel Victima { get; set; } = new();

    public bool VictimaRequiereProteccion { get; set; }

    [MaxLength(1000)]
    public string? ObservacionesVictima { get; set; }


    // =========================================================
    // PRESUNTO AGRESOR
    // =========================================================

    /// <summary>
    /// El agresor puede ser desconocido al registrar el caso.
    /// </summary>
    public bool IncluirPresuntoAgresor { get; set; }

    public PersonaRegistroViewModel PresuntoAgresor { get; set; } = new();

    [MaxLength(100)]
    public string? RelacionConVictima { get; set; }


    // =========================================================
    // OPCIONES PARA LOS DESPLEGABLES
    // =========================================================

    public List<OpcionCatalogoViewModel> TiposDocumento { get; set; } = [];

    public List<OpcionCatalogoViewModel> Provincias { get; set; } = [];

    public List<OpcionCatalogoViewModel> TiposViolencia { get; set; } = [];


    // =========================================================
    // VALIDACIÓN DE NEGOCIO BÁSICA DEL FORMULARIO
    // =========================================================

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (TiposViolenciaIds.Count == 0)
        {
            yield return new ValidationResult(
                "Debe seleccionar al menos un tipo de violencia.",
                [nameof(TiposViolenciaIds)]);
        }


        // La víctima principal sí es obligatoria.
        if (string.IsNullOrWhiteSpace(Victima.Nombres))
        {
            yield return new ValidationResult(
                "Debe indicar el nombre de la víctima.",
                ["Victima.Nombres"]);
        }

        if (string.IsNullOrWhiteSpace(Victima.Apellidos))
        {
            yield return new ValidationResult(
                "Debe indicar los apellidos de la víctima.",
                ["Victima.Apellidos"]);
        }


        /*
         * Solo exigimos datos del presunto agresor si el usuario
         * indica que existe uno identificado.
         */
        if (IncluirPresuntoAgresor)
        {
            if (string.IsNullOrWhiteSpace(PresuntoAgresor.Nombres))
            {
                yield return new ValidationResult(
                    "Debe indicar el nombre del presunto agresor.",
                    ["PresuntoAgresor.Nombres"]);
            }

            if (string.IsNullOrWhiteSpace(PresuntoAgresor.Apellidos))
            {
                yield return new ValidationResult(
                    "Debe indicar los apellidos del presunto agresor.",
                    ["PresuntoAgresor.Apellidos"]);
            }
        }
    }
}