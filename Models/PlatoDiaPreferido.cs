// Models/PlatoDiaPreferido.cs
namespace CasaApp.Api.Models
{
    public class PlatoDiaPreferido
    {
        public int PlatoId { get; set; }
        public DiaSemana Dia { get; set; }

        public virtual Plato? Plato { get; set; }
    }
}