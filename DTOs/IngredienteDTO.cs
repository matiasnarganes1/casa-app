using System.ComponentModel.DataAnnotations;
public class IngredienteDto
{
    [Required(ErrorMessage = "El ID del ingrediente es obligatorio.")]
    public int IngredienteId { get; set; }
    
    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "La cantidad debe ser un número entero positivo.")]
    public int Cantidad { get; set; } = 0;

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    [StringLength(50, ErrorMessage = "La unidad de medida no puede exceder los 50 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La unidad de medida solo puede contener letras y espacios.")]
    public string UnidadMedida { get; set; } = string.Empty;
}