using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreatePlatoDto
{
    [Required(ErrorMessage = "El nombre del plato es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre del plato no puede exceder los 100 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del plato solo puede contener letras y espacios.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe incluir al menos un ingrediente.")]
    public List<IngredienteDto> Ingredientes { get; set; } = new List<IngredienteDto>();
}