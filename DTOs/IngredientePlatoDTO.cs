namespace CasaApp.Api.DTOs;

public class IngredienteEnPlatoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; } = 0;
    public string UnidadMedida { get; set; } = string.Empty;
}