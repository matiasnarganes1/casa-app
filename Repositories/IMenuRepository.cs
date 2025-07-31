using CasaApp.Api.DTOs;
using CasaApp.Api.Models;

namespace CasaApp.Api.Repositories;

public interface IMenuRepository
{
    Task<IEnumerable<Plato>> GetAllPlatosAsync();
    Task<PlatoDto> CreatePlatoAsync(CreatePlatoDto plato);
    Task<Plato?> GetPlatoByName(string name);
    Task<Plato?> GetPlatoWithIngredientesAsync(int id);
    Task<bool> AddIngredienteToPlatoAsync(int platoId, CreateIngredienteEnPlatoDto ingrediente);
    Task<bool> UpdatePlatoAsync(int id, Plato plato);
    Task<bool> DeletePlatoAsync(int id);
    Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync();
    Task<IngredienteDto> CreateIngredienteAsync(CreateIngredienteDto ingrediente);
    Task<Ingrediente?> GetIngredienteAsync(int id);
    Task<bool> UpdateIngredienteAsync(int id, Ingrediente ingrediente);
    Task<bool> DeleteIngredienteAsync(int id);
}