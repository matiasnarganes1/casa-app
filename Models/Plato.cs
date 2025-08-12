namespace CasaApp.Api.Models;

public class Plato
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Preferencia { get; set; }
    public ICollection<PlatoIngrediente> Ingredientes { get; set; } = new List<PlatoIngrediente>();
    public ICollection<PlatoDiaPreferido> DiasPreferidos { get; set; } = new List<PlatoDiaPreferido>();
}