namespace CasaApp.Api.Models;
public class Menu
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public ICollection<PlatoMenu> Platos { get; set; } = new List<PlatoMenu>();
}