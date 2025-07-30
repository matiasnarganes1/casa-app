using CasaApp.Api.DTOs;
using CasaApp.Api.Models;

namespace CasaApp.Api.Repositories;

public interface IMenuRepository
{
    Task<IEnumerable<Plato>> GetAllPlatosAsync();
    Task<PlatoDto> CreatePlatoAsync(CreatePlatoDto plato);
    Task<Plato?> GetPlatoWithIngredientesAsync(int id);
    Task<bool> AddIngredienteToPlatoAsync(int platoId, PlatoIngrediente ingrediente);
    Task<bool> UpdatePlatoAsync(int id, Plato plato);
    Task<bool> DeletePlatoAsync(int id);
    Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync();
    Task<IngredienteDto> CreateIngredienteAsync(CreateIngredienteDto ingrediente);
    Task<Ingrediente?> GetIngredienteAsync(int id);
    Task<bool> UpdateIngredienteAsync(int id, Ingrediente ingrediente);
    Task<bool> DeleteIngredienteAsync(int id);
}