namespace CasaApp.Api.DTOs;

public class PlatoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Preferencia { get; set; }
    public List<string>? DiasPreferidos { get; set; }
    public bool OneShot { get; set; }
    public List<IngredienteEnPlatoDto> Ingredientes { get; set; } = new();
}