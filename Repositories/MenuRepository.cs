using System.Data.SqlTypes;
using CasaApp.Api.Data;
using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaApp.Api.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly CasaDbContext _context;

    public MenuRepository(CasaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateMenuAsync(CreateMenuDto menu)
    {
        try
        {
            foreach (var plato in menu.Platos)
            {
                var existingPlato = await _context.Platos.FindAsync(plato.PlatoId);
                if (existingPlato == null)
                    throw new Exception($"El plato con ID {plato.PlatoId} no existe");
            }

            Menu menuEntity = new Menu
            {
                Nombre = menu.Nombre,
                Tipo = menu.Tipo,
                FechaInicio = menu.FechaInicio,
                FechaFin = menu.FechaFin,
                Platos = menu.Platos.Select(p => new PlatoMenu
                {
                    PlatoId = p.PlatoId,
                    Dia = p.Dia,
                    Momento = p.Momento
                }).ToList()
            };

            _context.Menus.Add(menuEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el menú: " + ex.Message, ex);
        }
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus
            .Include(m => m.Platos)
            .ThenInclude(pm => pm.Plato)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null) return null;

        MenuDto menuDto = new MenuDto
        {
            Id = menu.Id,
            Nombre = menu.Nombre,
            Tipo = menu.Tipo.ToString(),
            FechaInicio = menu.FechaInicio,
            FechaFin = menu.FechaFin,
            Platos = menu.Platos.Select(p => new PlatoEnMenuDto
            {
                PlatoId = p.PlatoId,
                Nombre = p.Plato.Nombre,
                Dia = p.Dia,
                Momento = p.Momento.ToString()
            }).ToList()
        };

        return menuDto;
    }

    public async Task<Menu?> GetMenuByNameAsync(string name)
    {
        return await _context.Menus.FirstOrDefaultAsync(m => m.Nombre == name);
    }

    public async Task<List<MenuDto>> GetAllAsync()
    {
        var menuList = await _context.Menus
            .Include(m => m.Platos)
            .ThenInclude(pm => pm.Plato)
            .ToListAsync();

        List<MenuDto> menuDtos = new List<MenuDto>();
        foreach (var menu in menuList)
        {
            MenuDto menuDto = new MenuDto
            {
                Id = menu.Id,
                Nombre = menu.Nombre,
                Tipo = menu.Tipo.ToString(),
                FechaInicio = menu.FechaInicio,
                FechaFin = menu.FechaFin,
                Platos = menu.Platos.Select(p => new PlatoEnMenuDto
                {
                    PlatoId = p.PlatoId,
                    Nombre = p.Plato.Nombre,
                    Dia = p.Dia,
                    Momento = p.Momento.ToString()
                }).ToList()
            };
            menuDtos.Add(menuDto);

        }
        return menuDtos;
    }

    public async Task<IEnumerable<PlatoDto>> GetAllPlatosAsync()
    {
        var platos = await _context.Platos.Include(p => p.Ingredientes).ThenInclude(i => i.Ingrediente).ToListAsync();

        return platos.Select(p => new PlatoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Ingredientes = p.Ingredientes.Select(pi => new IngredienteEnPlatoDto
            {
                Nombre = pi.Ingrediente!.Nombre,
                Cantidad = $"{pi.Cantidad} {pi.UnidadMedida}"
            }).ToList()
        });
    }

    public async Task<bool> CreatePlatoAsync(CreatePlatoDto plato)
    {
        try
        {
            Plato platoEntity = new Plato
            {
                Nombre = plato.Nombre,
            };

            _context.Platos.Add(platoEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id)
    {
        var plato = await _context.Platos
            .Include(p => p.Ingredientes)
                .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plato == null) return null;

        PlatoDto platoDto = new PlatoDto
        {
            Id = plato.Id,
            Nombre = plato.Nombre,
            Ingredientes = plato.Ingredientes.Select(pi => new IngredienteEnPlatoDto
            {
                Nombre = pi.Ingrediente!.Nombre,
                Cantidad = $"{pi.Cantidad} {pi.UnidadMedida}"
            }).ToList()
        };

        return platoDto;
    }

    public async Task<Plato?> GetPlatoByName(string name)
    {
        return await _context.Platos.FirstOrDefaultAsync(p => p.Nombre == name);
    }

    public async Task<bool> AddIngredienteToPlatoAsync(int platoId, CreateIngredienteEnPlatoDto ingrediente)
    {
        PlatoIngrediente ingredienteEntity = new PlatoIngrediente
        {
            PlatoId = platoId,
            IngredienteId = ingrediente.IngredienteId,
            Cantidad = ingrediente.Cantidad,
            UnidadMedida = ingrediente.UnidadMedida
        };

        ingredienteEntity.PlatoId = platoId;
        _context.PlatoIngredientes.Add(ingredienteEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePlatoAsync(int id, CreatePlatoDto plato)
    {
        var existingPlato = await _context.Platos.FindAsync(id);
        if (existingPlato == null) return false;

        existingPlato.Nombre = plato.Nombre;

        _context.Platos.Update(existingPlato);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePlatoAsync(int id)
    {
        var plato = await _context.Platos.FindAsync(id);
        if (plato == null) return false;

        _context.Platos.Remove(plato);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<IngredienteDto>> GetAllIngredientesAsync()
    {
        var ingredientes = await _context.Ingredientes.ToListAsync();

        return ingredientes.Select(i => new IngredienteDto
        {
            Id = i.Id,
            Nombre = i.Nombre
        });
    }

    public async Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente)
    {
        try
        {
            var exists = await _context.Ingredientes.AnyAsync(i => i.Nombre == ingrediente.Nombre);

            if (exists)
                throw new Exception("El ingrediente ya existe");

            Ingrediente ingredienteEntity = new Ingrediente
            {
                Nombre = ingrediente.Nombre
            };
            _context.Ingredientes.Add(ingredienteEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

    }

    public async Task<IngredienteDto?> GetIngredienteAsync(int id)
    {
        var ingrediente = await _context.Ingredientes.FirstOrDefaultAsync(p => p.Id == id);
        if (ingrediente == null) return null;
        return new IngredienteDto
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre
        };
    }

    public async Task<bool> UpdateIngredienteAsync(int id, CreateIngredienteDto ingrediente)
    {
        try
        {
            var existingIngrediente = await _context.Ingredientes.FindAsync(id);
            if (existingIngrediente == null) throw new Exception("El ingrediente no existe");
            var exists = await _context.Ingredientes.AnyAsync(i => i.Id != id && i.Nombre == ingrediente.Nombre);
            if (exists) throw new Exception("El ingrediente ya existe");

            existingIngrediente.Nombre = ingrediente.Nombre;

            _context.Ingredientes.Update(existingIngrediente);
            await _context.SaveChangesAsync();
            return true;

        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el ingrediente: " + ex.Message, ex);
        }
    }

    public async Task<bool> DeleteIngredienteAsync(int id)
    {
        var ingrediente = await _context.Ingredientes.FindAsync(id);
        if (ingrediente == null) return false;

        _context.Ingredientes.Remove(ingrediente);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId)
    {
        try
        {
            var plato = await _context.Platos.FindAsync(platoId);
            if (plato == null) throw new Exception("El plato no existe");

            var ingrediente = await _context.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente == null) throw new Exception("El ingrediente no existe");
            
            var platoIngrediente = await _context.PlatoIngredientes
                .FirstOrDefaultAsync(pi => pi.PlatoId == platoId && pi.IngredienteId == ingredienteId);

            if (platoIngrediente == null) return false;

            _context.PlatoIngredientes.Remove(platoIngrediente);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el ingrediente del plato: " + ex.Message, ex);
        }
    }
}