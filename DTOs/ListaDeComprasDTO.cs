public class ListaDeComprasDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public List<ListaDeComprasItemDto> Items { get; set; } = new();
}