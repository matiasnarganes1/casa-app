namespace CasaApp.Api.DTOs;
public class PlatoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<IngredienteEnPlatoDto> Ingredientes { get; set; } = new();
}