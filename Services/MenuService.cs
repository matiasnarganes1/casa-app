using System.Reflection.Metadata.Ecma335;
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
    public async Task<PlatoDto> CreatePlatoAsync(CreatePlatoDto plato)
    {
        try
        {
            var platoExists = await _repo.GetPlatoByName(plato.Nombre);
            if (platoExists != null) throw new Exception("El plato ya existe");

            foreach (var ingrediente in plato.Ingredientes)
            {
                var ingredienteExists = await _repo.GetIngredienteAsync(ingrediente.IngredienteId);
                if (ingredienteExists == null) throw new Exception($"El ingrediente no existe.");
            }
            return await _repo.CreatePlatoAsync(plato);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el plato: " + ex.Message, ex);
        }
    }

    public Task<Plato?> GetPlatoWithIngredientesAsync(int id) => _repo.GetPlatoWithIngredientesAsync(id);

    public async Task<bool> AddIngredienteToPlatoAsync(int platoId, CreateIngredienteEnPlatoDto ingrediente)
    {
        try
        {
            var platoExists = await _repo.GetPlatoWithIngredientesAsync(platoId);
            var ingredienteExists = await _repo.GetIngredienteAsync(ingrediente.IngredienteId);

            if (platoExists == null) throw new Exception("El plato no existe.");
            if (ingredienteExists == null) throw new Exception("El ingrediente no existe.");

            return await _repo.AddIngredienteToPlatoAsync(platoId, ingrediente);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al agregar un ingrediente al plato: " + ex.Message, ex);
        }
    }

    public Task<bool> UpdatePlatoAsync(int id, Plato plato) => _repo.UpdatePlatoAsync(id, plato);

    public Task<bool> DeletePlatoAsync(int id) => _repo.DeletePlatoAsync(id);
    public Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync() => _repo.GetAllIngredientesAsync();

    public Task<IngredienteDto> CreateIngredienteAsync(CreateIngredienteDto ingrediente) => _repo.CreateIngredienteAsync(ingrediente);

    public Task<Ingrediente?> GetIngredienteAsync(int id) => _repo.GetIngredienteAsync(id);

    public Task<bool> UpdateIngredienteAsync(int id, Ingrediente ingrediente) => _repo.UpdateIngredienteAsync(id, ingrediente);

    public Task<bool> DeleteIngredienteAsync(int id) => _repo.DeleteIngredienteAsync(id);
}