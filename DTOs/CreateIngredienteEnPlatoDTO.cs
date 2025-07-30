using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreateIngredienteEnPlatoDto
{
    [Required(ErrorMessage = "El ID del ingrediente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor que cero.")]
    public int IngredienteId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    [StringLength(50, ErrorMessage = "La unidad no puede exceder los 50 caracteres.")]
    [RegularExpression(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$", ErrorMessage = "Solo letras y espacios.")]
    public string UnidadMedida { get; set; } = string.Empty;
}