namespace CasaApp.Api.DTOs;

public class PlatoEnMenuDto
{
    public int PlatoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly Dia { get; set; }
    public string Momento { get; set; } = string.Empty;
}