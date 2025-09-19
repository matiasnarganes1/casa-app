// Models/PlatoMenu.cs
using System;

namespace CasaApp.Api.Models
{
    public class PlatoMenu
    {
        public int MenuId { get; set; }
        public int PlatoId { get; set; }
        public DateOnly Dia { get; set; }
        public MomentoDelDia Momento { get; set; }

        // navigations
        public virtual Menu? Menu { get; set; }
        public virtual Plato? Plato { get; set; }
    }
}