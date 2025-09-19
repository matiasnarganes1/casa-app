// Models/Plato.cs
using System.Collections.Generic;

namespace CasaApp.Api.Models
{
    public class Plato
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Preferencia { get; set; }
        public bool OneShot { get; set; }

        // navigation collections used by repo:
        // Plato.Ingredientes (collection of PlatoIngrediente)
        public virtual ICollection<PlatoIngrediente> Ingredientes { get; set; } = new HashSet<PlatoIngrediente>();

        // preferred days mapping
        public virtual ICollection<PlatoDiaPreferido> DiasPreferidos { get; set; } = new HashSet<PlatoDiaPreferido>();

        // for scaffolded DbContext relationships:
        public virtual ICollection<PlatoMenu> PlatoMenus { get; set; } = new HashSet<PlatoMenu>();
        // alternative plural used in some code: PlatosDiasPreferidos - DbContext has entity for that table.
    }
}