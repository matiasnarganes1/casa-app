using System.ComponentModel.DataAnnotations;

namespace CasaApp.Api.DTOs;

public class CreatePlatoEnMenuDto
{
    [Required(ErrorMessage = "El ID del plato es obligatorio.")]
    public int PlatoId { get; set; }

    [Required(ErrorMessage = "El día de la semana es obligatorio.")]
    public DateOnly Dia { get; set; }

    [Required(ErrorMessage = "El momento del día es obligatorio.")]
    [EnumDataType(typeof(MomentoDelDia))]
    public MomentoDelDia Momento { get; set; }
}