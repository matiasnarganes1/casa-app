namespace CasaApp.Api.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }

    public List<PlatoEnMenuDto> Platos { get; set; } = new();
    public ListaDeComprasDto ListaDeCompras { get; set; } = new();
}