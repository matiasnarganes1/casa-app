// Models/ListaDeCompras.cs
using System.Collections.Generic;

namespace CasaApp.Api.Models
{
    public class ListaDeCompras
    {
        public int Id { get; set; }
        public int MenuId { get; set; }

        public virtual Menu? Menu { get; set; }
        public virtual ICollection<ListaDeComprasItem> Items { get; set; } = new HashSet<ListaDeComprasItem>();
    }

    public class ListaDeComprasItem
    {
        public int Id { get; set; }
        public int ListaDeComprasId { get; set; }
        public int IngredienteId { get; set; }
        public int CantidadTotal { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;

        public virtual ListaDeCompras? ListaDeCompras { get; set; }
        public virtual Ingrediente? Ingrediente { get; set; }
    }
}