using CasaApp.Api.Models;

public class PlatoDiaPreferido
{
    public int PlatoId { get; set; }
    public DiaSemana Dia { get; set; }

    public Plato Plato { get; set; }
}