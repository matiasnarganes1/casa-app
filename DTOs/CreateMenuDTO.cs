using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreateMenuDto
{
    [Required(ErrorMessage = "El nombre del menú es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateOnly FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateOnly FechaFin { get; set; }
    [Required(ErrorMessage = "Elige al menos un momento del dia")]
    public List<MomentoDelDia> Momentos { get; set; }
}