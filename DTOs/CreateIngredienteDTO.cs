using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreateIngredienteDto
{
    [Required(ErrorMessage = "El nombre del ingrediente es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$", ErrorMessage = "Solo letras y espacios.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El tipo de ingrediente es obligatorio.")]
    public string Tipo { get; set; }
}