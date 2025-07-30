namespace CasaApp.Api.Models;

public class PlatoIngrediente
{
    public int PlatoId { get; set; }
    public Plato Plato { get; set; } = new Plato();

    public int IngredienteId { get; set; }
    public Ingrediente Ingrediente { get; set; } = new Ingrediente();

    public int Cantidad { get; set; } = 0;
    public string UnidadMedida { get; set; } = string.Empty;
}