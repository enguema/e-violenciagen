using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace e_violenciagen.ViewModels.Personas;

public class PersonaFormViewModel
{
    // =========================================================
    // IDENTIFICADOR
    // =========================================================

    /// <summary>
    /// Será null cuando estamos creando una persona.
    /// Tendrá valor cuando estamos editando.
    /// </summary>
    public Guid? Id { get; set; }


    // =========================================================
    // IDENTIFICACIÓN
    // =========================================================

    [Display(Name = "Tipo de documento")]
    public Guid? TipoDocumentoIdentidadId { get; set; }

    [Display(Name = "Número de documento")]
    [StringLength(
        50,
        ErrorMessage = "El número de documento no puede superar los 50 caracteres.")]
    public string? NumeroDocumento { get; set; }


    // =========================================================
    // DATOS PERSONALES
    // =========================================================

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(
        150,
        ErrorMessage = "Los nombres no pueden superar los 150 caracteres.")]
    public string Nombres { get; set; } = string.Empty;


    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(
        150,
        ErrorMessage = "Los apellidos no pueden superar los 150 caracteres.")]
    public string Apellidos { get; set; } = string.Empty;


    [Display(Name = "Fecha de nacimiento")]
    [DataType(DataType.Date)]
    public DateOnly? FechaNacimiento { get; set; }


    [StringLength(
        30,
        ErrorMessage = "El sexo no puede superar los 30 caracteres.")]
    public string? Sexo { get; set; }


    // =========================================================
    // FOTOGRAFÍA
    // =========================================================

    /// <summary>
    /// Archivo enviado por el formulario.
    ///
    /// IFormFile pertenece al ViewModel y NO a la entidad Persona.
    /// La validación del tipo, tamaño y contenido del archivo
    /// la realizaremos posteriormente en PersonaService.
    /// </summary>
    [Display(Name = "Fotografía")]
    public IFormFile? Foto { get; set; }


    /// <summary>
    /// Ruta de la fotografía que ya tiene la persona.
    ///
    /// Se utilizará principalmente durante la edición para
    /// mostrar la imagen actual sin obligar al usuario
    /// a seleccionar nuevamente una fotografía.
    /// </summary>
    public string? RutaFotoActual { get; set; }


    /// <summary>
    /// Permite que en la futura pantalla Edit el usuario
    /// pueda indicar expresamente que desea eliminar la foto.
    /// </summary>
    public bool EliminarFoto { get; set; }


    // =========================================================
    // CONTACTO
    // =========================================================

    [Display(Name = "Teléfono")]
    [StringLength(
        50,
        ErrorMessage = "El teléfono no puede superar los 50 caracteres.")]
    public string? Telefono { get; set; }


    [Display(Name = "Correo electrónico")]
    [EmailAddress(ErrorMessage = "Introduzca un correo electrónico válido.")]
    [StringLength(
        200,
        ErrorMessage = "El correo electrónico no puede superar los 200 caracteres.")]
    public string? Email { get; set; }


    // =========================================================
    // DIRECCIÓN
    // =========================================================

    [StringLength(
        500,
        ErrorMessage = "La dirección no puede superar los 500 caracteres.")]
    public string? Direccion { get; set; }


    [Display(Name = "Barrio")]
    public Guid? BarrioId { get; set; }
}