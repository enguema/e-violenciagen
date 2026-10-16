using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;

public class QuitarParticipanteActuacionViewModel
{
    [Required]
    public Guid CasoId { get; set; }

    [Required]
    public Guid ActuacionId { get; set; }

    [Required]
    public Guid UsuarioId { get; set; }
}