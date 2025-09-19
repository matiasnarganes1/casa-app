using System.Collections.Generic;

namespace CasaApp.Api.DTOs
{
    public class PlatoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Preferencia { get; set; }
        public bool OneShot { get; set; }
        public List<IngredienteEnPlatoDto> Ingredientes { get; set; } = new();
        public List<string> DiasPreferidos { get; set; } = new();
    }

    public class IngredienteEnPlatoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
    }
}