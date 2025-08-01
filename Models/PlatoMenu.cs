namespace CasaApp.Api.Models;
public class PlatoMenu
{
    public int MenuId { get; set; }
    public Menu Menu { get; set; }

    public int PlatoId { get; set; }
    public Plato Plato { get; set; }

    public DateOnly Dia { get; set; }
    public MomentoDelDia Momento { get; set; }
}