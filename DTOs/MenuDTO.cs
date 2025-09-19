// DTOs/MenuDto.cs
using System;
using System.Collections.Generic;

namespace CasaApp.Api.DTOs
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public List<PlatoEnMenuDto> Platos { get; set; } = new();
        public ListaDeComprasDto? ListaDeCompras { get; set; }
    }

    public class PlatoEnMenuDto
    {
        public int PlatoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly Dia { get; set; }
        public string Momento { get; set; } = string.Empty;
    }

    public class ListaDeComprasDto
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public List<ListaDeComprasItemDto> Items { get; set; } = new();
    }

    public class ListaDeComprasItemDto
    {
        public int IngredienteId { get; set; }
        public string IngredienteNombre { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}