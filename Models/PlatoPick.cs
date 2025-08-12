namespace CasaApp.Api.Models;
    public class PlatoPick
{
    public int Id { get; set; }
    public int Preferencia { get; set; }
    public HashSet<DayOfWeek> Preferidos { get; set; } = new();
}