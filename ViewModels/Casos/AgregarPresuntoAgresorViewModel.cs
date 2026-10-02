using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;
/// <summary>
    /// Datos necesarios para vincular una persona existente
    /// como presunto agresor de un caso.
    /// </summary>
    public class AgregarPresuntoAgresorViewModel
    {
        [Required]
        public Guid CasoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una persona.")]
        public Guid? PersonaId { get; set; }
    }