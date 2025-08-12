using System.ComponentModel.DataAnnotations;
public class IngredienteDto
{
    [Required(ErrorMessage = "El ID del ingrediente es obligatorio.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del ingrediente es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre del ingrediente no puede exceder los 100 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del ingrediente solo puede contener letras y espacios.")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El tipo de ingrediente es obligatorio.")]
    public string Tipo { get; set; }
}