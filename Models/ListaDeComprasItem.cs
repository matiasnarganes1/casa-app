namespace CasaApp.Api.Models;
public class ListaDeComprasItem
{
    public int Id { get; set; }

    public int ListaDeComprasId { get; set; }
    public ListaDeCompras ListaDeCompras { get; set; } = null!;

    public int IngredienteId { get; set; }
    public Ingrediente Ingrediente { get; set; }

    public int CantidadTotal { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
}