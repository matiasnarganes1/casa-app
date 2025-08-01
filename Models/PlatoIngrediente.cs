namespace CasaApp.Api.Models;

public class PlatoIngrediente
{
    public int PlatoId { get; set; }
    public Plato? Plato { get; set; }

    public int IngredienteId { get; set; }
    public Ingrediente? Ingrediente { get; set; }

    public int Cantidad { get; set; } = 0;
    public string UnidadMedida { get; set; } = string.Empty;
}