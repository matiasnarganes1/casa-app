using CasaApp.Api.DTOs;
using CasaApp.Api.Models;

namespace CasaApp.Api.Repositories;

public interface IMenuRepository
{
    Task<Menu?> CreateMenuAsync(CreateMenuDto menu);
    Task<MenuDto?> GetByIdAsync(int id);
    Task<Menu?> GetMenuByNameAsync(string name);
    Task<List<MenuDto>> GetAllAsync();
    Task<bool> DeleteMenuAsync(int id);
    Task<IEnumerable<PlatoDto>> GetAllPlatosAsync();
    Task<PlatoDto> CreatePlatoAsync(PlatoDto plato);
    Task<Plato?> GetPlatoByName(string name);
    Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id);
    Task<bool> AddIngredienteToPlatoAsync(int platoId, CreateIngredienteEnPlatoDto ingrediente);
    Task<bool> UpdatePlatoAsync(int id, PlatoDto plato);
    Task<bool> DeletePlatoAsync(int id);
    Task<IEnumerable<IngredienteDto>> GetAllIngredientesAsync();
    Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente);
    Task<IngredienteDto?> GetIngredienteAsync(int id);
    Task<IngredienteDto?> GetIngredienteByNameAsync(string nombre);
    Task<bool> UpdateIngredienteAsync(int id, CreateIngredienteDto ingrediente);
    Task<bool> DeleteIngredienteAsync(int id);
    Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId);
    Task<Menu?> GetMenuWithPlatosAndIngredientesAsync(int menuId);
    Task UpsertShoppingListAsync(int menuId, IEnumerable<ListaDeComprasItem> items);
}