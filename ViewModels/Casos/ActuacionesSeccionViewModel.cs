using e_violenciagen.ViewModels.Casos;

namespace e_violenciagen.ViewModels.Casos;

    /// <summary>
    /// Modelo específico para la sección de actuaciones
    /// dentro del detalle de un caso.
    /// </summary>
    public class ActuacionesSeccionViewModel
    {
        /// <summary>
        /// Caso al que pertenecen las actuaciones.
        ///
        /// Se necesita para las operaciones POST que,
        /// después de ejecutarse, regresan a Caso/Details.
        /// </summary>
        public Guid CasoId { get; set; }

        /// <summary>
        /// Actuaciones registradas dentro del expediente.
        /// </summary>
        public List<CasoActuacionViewModel> Actuaciones { get; set; } = [];
    }
