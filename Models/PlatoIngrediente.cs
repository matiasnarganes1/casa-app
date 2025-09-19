namespace CasaApp.Api.Models
{
    public class PlatoIngrediente
    {
        // composite key: PlatoId + IngredienteId
        public int PlatoId { get; set; }
        public int IngredienteId { get; set; }

        public int Cantidad { get; set; }

        // navigations
        public virtual Plato? Plato { get; set; }
        public virtual Ingrediente? Ingrediente { get; set; }
    }
}