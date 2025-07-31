using AutoMapper;
using CasaApp.Api.Data;
using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaApp.Api.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly CasaDbContext _context;
    private readonly IMapper _mapper;

    public MenuRepository(CasaDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Plato>> GetAllPlatosAsync()
    {
        return await _context.Platos.ToListAsync();
    }

    public async Task<PlatoDto> CreatePlatoAsync(CreatePlatoDto plato)
    {
        try
        { 
            var platoEntity = _mapper.Map<Plato>(plato);

            _context.Platos.Add(platoEntity);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<PlatoDto>(platoEntity);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<Plato?> GetPlatoWithIngredientesAsync(int id)
    {
        return await _context.Platos
            .Include(p => p.Ingredientes)
                .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(p => p.Id == id);
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
            Cantidad = ingrediente.Cantidad
        };

        ingredienteEntity.PlatoId = platoId;
        _context.PlatoIngredientes.Add(ingredienteEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePlatoAsync(int id, Plato plato)
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

    public async Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync()
    {
        return await _context.Ingredientes.ToListAsync();
    }

    public async Task<IngredienteDto> CreateIngredienteAsync(CreateIngredienteDto ingrediente)
    {
        try
        {
            var exists = await _context.Ingredientes.AnyAsync(i => i.Nombre == ingrediente.Nombre);

            if (exists)
                throw new Exception("El ingrediente ya existe");

            var ingredienteEntity = _mapper.Map<Ingrediente>(ingrediente);
            _context.Ingredientes.Add(ingredienteEntity);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<IngredienteDto>(ingrediente);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

    }

    public async Task<Ingrediente?> GetIngredienteAsync(int id)
    {
        return await _context.Ingredientes.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> UpdateIngredienteAsync(int id, Ingrediente ingrediente)
    {
        var existingIngrediente = await _context.Ingredientes.FindAsync(id);
        if (existingIngrediente == null) return false;

        existingIngrediente.Nombre = ingrediente.Nombre;

        _context.Ingredientes.Update(existingIngrediente);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIngredienteAsync(int id)
    {
        var ingrediente = await _context.Ingredientes.FindAsync(id);
        if (ingrediente == null) return false;

        _context.Ingredientes.Remove(ingrediente);
        await _context.SaveChangesAsync();
        return true;
    }
}