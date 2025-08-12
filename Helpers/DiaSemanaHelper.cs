public static class DiaSemanaHelper
{
    private static readonly Dictionary<string, DiaSemana> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Lunes"] = DiaSemana.Lunes,
            ["Martes"] = DiaSemana.Martes,
            ["Miércoles"] = DiaSemana.Miércoles,
            ["Miercoles"] = DiaSemana.Miércoles, // por si viene sin tilde
            ["Jueves"] = DiaSemana.Jueves,
            ["Viernes"] = DiaSemana.Viernes,
            ["Sábado"] = DiaSemana.Sábado,
            ["Sabado"] = DiaSemana.Sábado,
            ["Domingo"] = DiaSemana.Domingo
        };

    public static DiaSemana? Parse(string? dia)
        => dia != null && Map.TryGetValue(dia.Trim(), out var val) ? val : null;

    public static string ToStringLabel(this DiaSemana d)
        => d switch
        {
            DiaSemana.Lunes => "Lunes",
            DiaSemana.Martes => "Martes",
            DiaSemana.Miércoles => "Miércoles",
            DiaSemana.Jueves => "Jueves",
            DiaSemana.Viernes => "Viernes",
            DiaSemana.Sábado => "Sábado",
            DiaSemana.Domingo => "Domingo",
            _ => ""
        };
    public static DayOfWeek MapDiaSemanaToDayOfWeek(DiaSemana dia) => dia switch
    {
        DiaSemana.Lunes => DayOfWeek.Monday,
        DiaSemana.Martes => DayOfWeek.Tuesday,
        DiaSemana.Miércoles => DayOfWeek.Wednesday,
        DiaSemana.Jueves => DayOfWeek.Thursday,
        DiaSemana.Viernes => DayOfWeek.Friday,
        DiaSemana.Sábado => DayOfWeek.Saturday,
        DiaSemana.Domingo => DayOfWeek.Sunday,
        _ => DayOfWeek.Monday
    };
}