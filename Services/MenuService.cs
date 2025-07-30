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

    public Task<IEnumerable<Plato>> GetAllPlatosAsync() => _repo.GetAllPlatosAsync();

    public Task<PlatoDto> CreatePlatoAsync(CreatePlatoDto plato) => _repo.CreatePlatoAsync(plato);

    public Task<Plato?> GetPlatoWithIngredientesAsync(int id) => _repo.GetPlatoWithIngredientesAsync(id);

    public Task<bool> AddIngredienteToPlatoAsync(int platoId, PlatoIngrediente ingrediente)
        => _repo.AddIngredienteToPlatoAsync(platoId, ingrediente);

    public Task<bool> UpdatePlatoAsync(int id, Plato plato) => _repo.UpdatePlatoAsync(id, plato);

    public Task<bool> DeletePlatoAsync(int id) => _repo.DeletePlatoAsync(id);
    public Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync() => _repo.GetAllIngredientesAsync();

    public Task<Ingrediente> CreateIngredienteAsync(Ingrediente ingrediente) => _repo.CreateIngredienteAsync(ingrediente);

    public Task<Ingrediente?> GetIngredienteAsync(int id) => _repo.GetIngredienteAsync(id);

    public Task<bool> UpdateIngredienteAsync(int id, Ingrediente ingrediente) => _repo.UpdateIngredienteAsync(id, ingrediente);
    
    public Task<bool> DeleteIngredienteAsync(int id) => _repo.DeleteIngredienteAsync(id);
}