using System.ComponentModel.DataAnnotations;

namespace e_violenciagen.ViewModels.Casos;

public class AgregarParticipanteActuacionViewModel
{
    [Required]
    public Guid CasoId { get; set; }

    [Required]
    public Guid ActuacionId { get; set; }

    [Required(ErrorMessage =
        "Debe seleccionar un participante.")]
    public Guid UsuarioId { get; set; }
}