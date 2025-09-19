// Models/Menu.cs
using System;
using System.Collections.Generic;

namespace CasaApp.Api.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }

        // navegación hacia plato-menu (la relación real en db es PlatoMenu)
        // tu repository usa "Platos" como colección de PlatoMenu — mantenla:
        public virtual ICollection<PlatoMenu> Platos { get; set; } = new HashSet<PlatoMenu>();

        // lista de compras mapping (repos usa ListaDeCompras)
        public virtual ListaDeCompras? ListaDeCompras { get; set; }
    }
}