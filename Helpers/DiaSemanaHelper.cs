// Models/DiaSemanaHelper.cs
using System;
using System.Globalization;

namespace CasaApp.Api.Models
{
    public static class DiaSemanaHelper
    {
        public static DiaSemana? Parse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (Enum.TryParse<DiaSemana>(s, true, out var v)) return v;
            // allow numeric parsing
            if (int.TryParse(s, out var i) && Enum.IsDefined(typeof(DiaSemana), i))
                return (DiaSemana)i;
            return null;
        }

        public static string ToStringLabel(this DiaSemana d)
        {
            return d switch
            {
                DiaSemana.Domingo => "Dom",
                DiaSemana.Lunes => "Lun",
                DiaSemana.Martes => "Mar",
                DiaSemana.Miercoles => "Mié",
                DiaSemana.Jueves => "Jue",
                DiaSemana.Viernes => "Vie",
                DiaSemana.Sabado => "Sáb",
                _ => d.ToString()
            };
        }

        public static DayOfWeek MapDiaSemanaToDayOfWeek(int dia) => dia switch
        {
            0 => DayOfWeek.Sunday,
            1 => DayOfWeek.Monday,
            2 => DayOfWeek.Tuesday,
            3 => DayOfWeek.Wednesday,
            4 => DayOfWeek.Thursday,
            5 => DayOfWeek.Friday,
            6 => DayOfWeek.Saturday,
            _ => DayOfWeek.Sunday
        };

        public static DayOfWeek MapDiaSemanaToDayOfWeek(DiaSemana d) => d switch
        {
            DiaSemana.Domingo => DayOfWeek.Sunday,
            DiaSemana.Lunes => DayOfWeek.Monday,
            DiaSemana.Martes => DayOfWeek.Tuesday,
            DiaSemana.Miercoles => DayOfWeek.Wednesday,
            DiaSemana.Jueves => DayOfWeek.Thursday,
            DiaSemana.Viernes => DayOfWeek.Friday,
            DiaSemana.Sabado => DayOfWeek.Saturday,
            _ => DayOfWeek.Sunday
        };

        public static DayOfWeek MapDiaSemanaToDayOfWeek(object obj)
        {
            if (obj is int i) return MapDiaSemanaToDayOfWeek(i);
            if (obj is DiaSemana d) return MapDiaSemanaToDayOfWeek(d);
            return DayOfWeek.Sunday;
        }
    }
}