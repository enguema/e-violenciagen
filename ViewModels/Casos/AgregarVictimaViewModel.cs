using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;
/// <summary>
    /// Datos necesarios para vincular una persona
    /// existente como víctima de un caso.
    /// </summary>
    public class AgregarVictimaViewModel
    {
        [Required]
        public Guid CasoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una persona.")]
        public Guid? PersonaId { get; set; }
    }