namespace CasaApp.Api.Models;

public class ListaDeCompras
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public ICollection<ListaDeComprasItem> Items { get; set; } = new List<ListaDeComprasItem>();
}