using System.Reflection.Metadata.Ecma335;
using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using CasaApp.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CasaApp.Api.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _repo;

    public MenuService(IMenuRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> CreateMenuAsync(CreateMenuDto dto)
    {
        try
        {
            var existingMenu = await _repo.GetMenuByNameAsync(dto.Nombre);

            if (existingMenu != null)
                throw new Exception("El menú ya existe");

            await _repo.CreateMenuAsync(dto);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el menú: " + ex.Message, ex);
        }

    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = await _repo.GetByIdAsync(id);

        if (menu == null) return null;

        MenuDto menuDto = new MenuDto
        {
            Id = menu.Id,
            Nombre = menu.Nombre,
            Tipo = menu.Tipo,
            FechaInicio = menu.FechaInicio,
            FechaFin = menu.FechaFin,
            Platos = menu.Platos.Select(p => new PlatoEnMenuDto
            {
                PlatoId = p.PlatoId,
                Nombre = p.Nombre,
                Dia = p.Dia,
                Momento = p.Momento
            }).ToList()
        };

        return menuDto;
    }

    public async Task<List<MenuDto>> GetAllMenusAsync()
    {
        var menus = await _repo.GetAllAsync();

        return menus;
    }

    public Task<IEnumerable<PlatoDto>> GetAllPlatosAsync() => _repo.GetAllPlatosAsync();
    public async Task<bool> CreatePlatoAsync(CreatePlatoDto plato)
    {
        try
        {
            var platoExists = await _repo.GetPlatoByName(plato.Nombre);
            if (platoExists != null) throw new Exception("El plato ya existe");

            var created = await _repo.CreatePlatoAsync(plato);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el plato: " + ex.Message, ex);
        }
    }

    public Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id) => _repo.GetPlatoWithIngredientesAsync(id);

    public async Task<bool> AddIngredienteToPlatoAsync(int platoId, List<CreateIngredienteEnPlatoDto> ingredientes)
    {
        try
        {
            if (ingredientes == null || !ingredientes.Any())
                throw new ArgumentException("La lista de ingredientes no puede estar vacía.");

            var platoExists = await _repo.GetPlatoWithIngredientesAsync(platoId);
            if (platoExists == null) throw new Exception("El plato no existe.");

            foreach (var item in ingredientes)
            {
                var ingredienteExists = await _repo.GetIngredienteAsync(item.IngredienteId);
                if (ingredienteExists == null)
                    throw new Exception($"El ingrediente con ID {item.IngredienteId} no existe.");

                await _repo.AddIngredienteToPlatoAsync(platoId, item);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al agregar ingredientes al plato: " + ex.Message, ex);
        }
    }

    public Task<bool> UpdatePlatoAsync(int id, CreatePlatoDto plato) => _repo.UpdatePlatoAsync(id, plato);

    public Task<bool> DeletePlatoAsync(int id) => _repo.DeletePlatoAsync(id);
    public Task<IEnumerable<IngredienteDto>> GetAllIngredientesAsync() => _repo.GetAllIngredientesAsync();

    public Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente) => _repo.CreateIngredienteAsync(ingrediente);

    public Task<IngredienteDto?> GetIngredienteAsync(int id) => _repo.GetIngredienteAsync(id);

    public Task<bool> UpdateIngredienteAsync(int id, CreateIngredienteDto ingrediente) => _repo.UpdateIngredienteAsync(id, ingrediente);

    public Task<bool> DeleteIngredienteAsync(int id) => _repo.DeleteIngredienteAsync(id);

    public Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId) 
        => _repo.DeleteIngredienteFromPlatoAsync(platoId, ingredienteId);
}