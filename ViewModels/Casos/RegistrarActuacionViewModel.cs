using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;
public class RegistrarActuacionViewModel
    {
        [Required]
        public Guid CasoId { get; set; }


        // =============================================
        // CLASIFICACIÓN
        // =============================================

        [Required(ErrorMessage =
            "Debe seleccionar el tipo de actuación.")]
        public Guid? TipoActuacionId { get; set; }


        // =============================================
        // INSTITUCIÓN
        // =============================================

        [Required(ErrorMessage =
            "Debe seleccionar la institución responsable.")]
        public Guid? InstitucionId { get; set; }


        /// <summary>
        /// Es opcional.
        /// </summary>
        public Guid? UnidadOrganizativaId { get; set; }


        // =============================================
        // FECHA
        // =============================================

        [Required(ErrorMessage =
            "Debe indicar la fecha de la actuación.")]
        public DateTime FechaActuacion { get; set; }


        // =============================================
        // CONTENIDO
        // =============================================

        [Required(ErrorMessage =
            "Debe indicar un título para la actuación.")]
        [StringLength(
            200,
            ErrorMessage =
                "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; }
            = string.Empty;


        [StringLength(
            4000,
            ErrorMessage =
                "La descripción no puede superar los 4000 caracteres.")]
        public string? Descripcion { get; set; }


        [StringLength(
            2000,
            ErrorMessage =
                "El resultado no puede superar los 2000 caracteres.")]
        public string? Resultado { get; set; }
    }