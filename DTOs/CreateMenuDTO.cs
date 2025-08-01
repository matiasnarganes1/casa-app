using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreateMenuDto
{
    [Required(ErrorMessage = "El nombre del menú es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de menú es obligatorio.")]
    [EnumDataType(typeof(TipoMenu), ErrorMessage = "Tipo de menú inválido.")]
    public TipoMenu Tipo { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateOnly FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateOnly FechaFin { get; set; }

    [MinLength(1, ErrorMessage = "Debe incluir al menos un plato.")]
    public List<CreatePlatoEnMenuDto> Platos { get; set; } = new();
}