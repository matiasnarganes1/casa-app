// Models/Ingrediente.cs
using System.Collections.Generic;

namespace CasaApp.Api.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoIngrediente Tipo { get; set; }
        public TipoUnidadMedida UnidadMedida { get; set; }
        public virtual ICollection<PlatoIngrediente> PlatoIngredientes { get; set; } = new HashSet<PlatoIngrediente>();
        public virtual ICollection<ListaDeComprasItem> ListaDeComprasItems { get; set; } = new HashSet<ListaDeComprasItem>();
    }
}