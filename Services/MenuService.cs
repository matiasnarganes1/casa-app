using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using CasaApp.Api.Repositories;

namespace CasaApp.Api.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _repo;

    public MenuService(IMenuRepository repo)
    {
        _repo = repo;
    }

    public async Task<MenuDto?> CreateMenuAsync(CreateMenuDto dto)
    {
        var existingMenu = await _repo.GetMenuByNameAsync(dto.Nombre);
        if (existingMenu is not null) throw new InvalidOperationException("El menú ya existe.");

        var created = await _repo.CreateMenuAsync(dto);
        if (created is null) return null;

        var platos = created.Platos.ToList();

        var lunchesToSkip = new HashSet<(DateOnly day, int platoId)>(
            platos
                .Where(pm => pm.Momento == MomentoDelDia.Cena && !pm.Plato.OneShot)
                .SelectMany(dinner =>
                    platos.Where(x =>
                        x.Momento == MomentoDelDia.Almuerzo &&
                        x.PlatoId == dinner.PlatoId &&
                        x.Dia == dinner.Dia.AddDays(1))
                    .Select(lunch => (lunch.Dia, lunch.PlatoId))
                )
        );

        var agregados = platos
            .Where(pm => !(pm.Momento == MomentoDelDia.Almuerzo && lunchesToSkip.Contains((pm.Dia, pm.PlatoId))))
            .SelectMany(pm => pm.Plato.Ingredientes)
            .GroupBy(pi => new { pi.IngredienteId, pi.UnidadMedida })
            .Select(g => new ListaDeComprasItem
            {
                IngredienteId = g.Key.IngredienteId,
                UnidadMedida = g.Key.UnidadMedida,
                CantidadTotal = g.Sum(x => x.Cantidad)
            })
            .ToList();

        await _repo.UpsertShoppingListAsync(created.Id, agregados);

        var dtoCreated = await GetMenuByIdAsync(created.Id);
        return dtoCreated;
    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = await _repo.GetByIdAsync(id);
        return menu;
    }

    public Task<List<MenuDto>> GetAllMenusAsync() => _repo.GetAllAsync();

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var exists = await _repo.GetByIdAsync(id);
        if (exists is null) return false;
        return await _repo.DeleteMenuAsync(id);
    }

    public Task<IEnumerable<PlatoDto>> GetAllPlatosAsync() => _repo.GetAllPlatosAsync();

    public async Task<PlatoDto> CreatePlatoAsync(PlatoDto plato)
    {
        var exists = await _repo.GetPlatoByName(plato.Nombre);
        if (exists is not null) throw new InvalidOperationException("El plato ya existe.");
        return await _repo.CreatePlatoAsync(plato);
    }

    public Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id) => _repo.GetPlatoWithIngredientesAsync(id);

    public async Task<bool> AddIngredienteToPlatoAsync(int platoId, List<CreateIngredienteEnPlatoDto> ingredientes)
    {
        if (ingredientes is null || ingredientes.Count == 0) throw new ArgumentException("La lista de ingredientes no puede estar vacía.");
        var platoExists = await _repo.GetPlatoWithIngredientesAsync(platoId);
        if (platoExists is null) throw new InvalidOperationException("El plato no existe.");

        foreach (var i in ingredientes)
        {
            var ing = await _repo.GetIngredienteAsync(i.IngredienteId);
            if (ing is null) throw new InvalidOperationException($"El ingrediente con ID {i.IngredienteId} no existe.");
            await _repo.AddIngredienteToPlatoAsync(platoId, i);
        }
        return true;
    }

    public Task<bool> UpdatePlatoAsync(int id, PlatoDto plato) => _repo.UpdatePlatoAsync(id, plato);

    public Task<bool> DeletePlatoAsync(int id) => _repo.DeletePlatoAsync(id);

    public Task<IEnumerable<IngredienteDto>> GetAllIngredientesAsync() => _repo.GetAllIngredientesAsync();

    public Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente) => _repo.CreateIngredienteAsync(ingrediente);

    public Task<IngredienteDto?> GetIngredienteAsync(int id) => _repo.GetIngredienteAsync(id);

    public Task<bool> UpdateIngredienteAsync(int id, CreateIngredienteDto ingrediente) => _repo.UpdateIngredienteAsync(id, ingrediente);

    public Task<bool> DeleteIngredienteAsync(int id) => _repo.DeleteIngredienteAsync(id);

    public Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId) => _repo.DeleteIngredienteFromPlatoAsync(platoId, ingredienteId);

    public Task<IngredienteDto?> GetIngredienteByNameAsync(string nombre) => _repo.GetIngredienteByNameAsync(nombre);
}