namespace CasaApp.Api.Models;

public class Ingrediente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<PlatoIngrediente> Platos { get; set; } = new List<PlatoIngrediente>();
}