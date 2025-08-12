using CasaApp.Api.DTOs;
using CasaApp.Api.Models;

namespace CasaApp.Api.Services;

public interface IMenuService
{
    Task<bool> CreateMenuAsync(CreateMenuDto dto);
    Task<MenuDto?> GetMenuByIdAsync(int id);
    Task<List<MenuDto>> GetAllMenusAsync();
    Task<bool> DeleteMenuAsync(int id);
    Task<IEnumerable<PlatoDto>> GetAllPlatosAsync();
    Task<PlatoDto> CreatePlatoAsync(PlatoDto plato);
    Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id);
    Task<bool> AddIngredienteToPlatoAsync(int platoId, List<CreateIngredienteEnPlatoDto> ingrediente);
    Task<bool> UpdatePlatoAsync(int id, PlatoDto plato);
    Task<bool> DeletePlatoAsync(int id);
    Task<IEnumerable<IngredienteDto>> GetAllIngredientesAsync();
    Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente);
    Task<IngredienteDto?> GetIngredienteAsync(int id);
    Task<bool> UpdateIngredienteAsync(int id, CreateIngredienteDto ingrediente);
    Task<bool> DeleteIngredienteAsync(int id);
    Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId);
}