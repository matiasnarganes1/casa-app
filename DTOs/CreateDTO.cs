namespace CasaApp.Api.DTOs
{
    public class CreateIngredienteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
    }

    public class CreateIngredienteEnPlatoDto
    {
        public int IngredienteId { get; set; }
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
    }

    public class CreateMenuDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public List<MomentoDelDia> Momentos { get; set; } = new();
    }
}