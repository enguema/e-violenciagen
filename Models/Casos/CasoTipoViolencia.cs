namespace e_violenciagen.Models;
/// <summary>
/// Entidad intermedia que relaciona un Caso con
/// uno o varios Tipos de Violencia.
///
/// Relación:
///
/// Caso N ----- N TipoViolencia
///
/// se transforma físicamente en:
///
/// Caso 1 ----- N CasoTipoViolencia N ----- 1 TipoViolencia
/// </summary>
public class CasoTipoViolencia
{
    //public Guid Id {get; set;}
    public Guid CasoId { get; set; }

    public Caso Caso { get; set; } = null!;


    public Guid TipoViolenciaId { get; set; }

    public TipoViolencia TipoViolencia { get; set; } = null!;
}