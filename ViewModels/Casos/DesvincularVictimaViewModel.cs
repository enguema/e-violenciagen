using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos
{
    public class DesvincularVictimaViewModel
    {
        [Required]
        public Guid CasoId { get; set; }

        [Required]
        public Guid PersonaId { get; set; }
    }
}
