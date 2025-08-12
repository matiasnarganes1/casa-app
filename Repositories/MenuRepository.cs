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
            if (menu == null) throw new ArgumentNullException(nameof(menu));

            var platosMeta = await _context.Platos
                .Select(p => new
                {
                    p.Id,
                    Preferencia = p.Preferencia,
                    Dias = p.DiasPreferidos.Select(d => d.Dia)
                })
                .ToListAsync();

            if (platosMeta.Count == 0) throw new InvalidOperationException("No hay platos disponibles para armar el menú.");

            var allPlatos = platosMeta
                .Select(p => new PlatoPick
                {
                    Id = p.Id,
                    Preferencia = p.Preferencia,
                    Preferidos = new HashSet<DayOfWeek>(p.Dias.Select(DiaSemanaHelper.MapDiaSemanaToDayOfWeek))
                })
                .OrderByDescending(x => x.Preferencia)
                .ToList();

            var menuEntity = new Menu
            {
                Nombre = menu.Nombre.Trim(),
                FechaInicio = menu.FechaInicio,
                FechaFin = menu.FechaFin
            };

            _context.Menus.Add(menuEntity);
            await _context.SaveChangesAsync();

            var usados = new HashSet<int>();
            var carryAlmuerzo = new Dictionary<DateOnly, int>();

            var rows = new List<PlatoMenu>();

            for (var dia = menu.FechaInicio; dia <= menu.FechaFin; dia = dia.AddDays(1))
            {
                foreach (var momento in menu.Momentos)
                {
                    if (momento == MomentoDelDia.Almuerzo && carryAlmuerzo.TryGetValue(dia, out var platoFromCena))
                    {
                        rows.Add(new PlatoMenu
                        {
                            MenuId = menuEntity.Id,
                            PlatoId = platoFromCena,
                            Dia = dia,
                            Momento = MomentoDelDia.Almuerzo
                        });
                        continue;
                    }

                    var platoId = PickPlatoForDay(allPlatos, usados, dia);
                    rows.Add(new PlatoMenu
                    {
                        MenuId = menuEntity.Id,
                        PlatoId = platoId,
                        Dia = dia,
                        Momento = momento
                    });

                    if (momento == MomentoDelDia.Cena)
                    {
                        var next = dia.AddDays(1);
                        if (IsSundayToThursday(dia) &&
                            next <= menu.FechaFin &&
                            menu.Momentos.Contains(MomentoDelDia.Almuerzo))
                        {
                            carryAlmuerzo[next] = platoId;
                        }
                    }
                }
            }

            await _context.PlatoMenus.AddRangeAsync(rows);
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

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return false;

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PlatoDto>> GetAllPlatosAsync()
    {
        var platos = await _context.Platos.Include(d => d.DiasPreferidos).Include(p => p.Ingredientes).ThenInclude(i => i.Ingrediente).ToListAsync();

        return platos.Select(p => new PlatoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Preferencia = p.Preferencia,
            Ingredientes = p.Ingredientes.Select(pi => new IngredienteEnPlatoDto
            {
                Id = pi.Ingrediente!.Id,
                Nombre = pi.Ingrediente!.Nombre,
                Cantidad = pi.Cantidad,
                UnidadMedida = pi.UnidadMedida,
            }).ToList(),
            DiasPreferidos = p.DiasPreferidos
                    .OrderBy(d => d.Dia)
                    .Select(d => d.Dia.ToStringLabel())
                    .ToList()
        });
    }

    public async Task<PlatoDto> CreatePlatoAsync(PlatoDto plato)
    {
        try
        {
            Plato platoEntity = new Plato
            {
                Nombre = plato.Nombre,
                Preferencia = plato.Preferencia,
            };

            var diasEnum = (plato.DiasPreferidos ?? new())
                .Select(DiaSemanaHelper.Parse)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var created = _context.Platos.Add(platoEntity).Entity;
            await _context.SaveChangesAsync();

            var newPlato = await GetPlatoByName(plato.Nombre);

            if (plato.Ingredientes != null)
            {
                List<PlatoIngrediente> ingredientes = new List<PlatoIngrediente>();
                foreach (var ingrediente in plato.Ingredientes)
                {
                    var ingredienteEntity = await _context.Ingredientes.FindAsync(ingrediente.Id);
                    if (ingredienteEntity == null)
                        throw new Exception($"El ingrediente con ID {ingrediente.Id} no existe");

                    ingredientes.Add(new PlatoIngrediente
                    {
                        PlatoId = newPlato!.Id,
                        IngredienteId = ingredienteEntity.Id,
                        Cantidad = ingrediente.Cantidad,
                        UnidadMedida = ingrediente.UnidadMedida
                    });
                    _context.PlatoIngredientes.AddRange(ingredientes);
                }
            }

            if (diasEnum.Count > 0)
            {
                var rowsDia = diasEnum.Select(d => new PlatoDiaPreferido
                {
                    PlatoId = newPlato!.Id,
                    Dia = d
                });
                await _context.PlatosDiasPreferidos.AddRangeAsync(rowsDia);
            }

            await _context.SaveChangesAsync();

            return new PlatoDto
            {
                Id = created.Id,
                Nombre = created.Nombre
            };
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
                Id = pi.Ingrediente!.Id,
                Nombre = pi.Ingrediente!.Nombre,
                Cantidad = pi.Cantidad,
                UnidadMedida = pi.UnidadMedida
            }).ToList()
        };

        return platoDto;
    }

    public async Task<Plato?> GetPlatoWithEntities(int id)
    {
        return await _context.Platos.Include(d => d.DiasPreferidos)
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
            Cantidad = ingrediente.Cantidad,
            UnidadMedida = ingrediente.UnidadMedida
        };

        ingredienteEntity.PlatoId = platoId;
        _context.PlatoIngredientes.Add(ingredienteEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePlatoAsync(int id, PlatoDto dto)
    {
        var plato = await GetPlatoWithEntities(id);

        if (plato == null)
            return false;

        if (!string.IsNullOrWhiteSpace(dto.Nombre))
            plato.Nombre = dto.Nombre.Trim();

        if (dto.Preferencia > 0)
            plato.Preferencia = dto.Preferencia;

        if (dto.Ingredientes != null)
        {
            var existentes = await _context.PlatoIngredientes
                .Where(pi => pi.PlatoId == id)
                .ToListAsync();

            _context.PlatoIngredientes.RemoveRange(existentes);

            var nuevosDistinct = dto.Ingredientes
                .GroupBy(i => i.Id)
                .Select(g =>
                {
                    var last = g.Last();
                    return new PlatoIngrediente
                    {
                        PlatoId = id,
                        IngredienteId = g.Key,
                        Cantidad = last.Cantidad,
                        UnidadMedida = last.UnidadMedida
                    };
                })
                .ToList();

            if (nuevosDistinct.Count > 0)
                await _context.PlatoIngredientes.AddRangeAsync(nuevosDistinct);
        }

        if (dto.DiasPreferidos != null)
        {
            var diasEnum = (dto.DiasPreferidos ?? Enumerable.Empty<string>())
                .Select(DiaSemanaHelper.Parse)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToHashSet();

            var actuales = await _context.PlatosDiasPreferidos
                .Where(x => x.PlatoId == id)
                .ToListAsync();

            var actualesSet = actuales.Select(x => x.Dia).ToHashSet();

            var paraAgregar = diasEnum
                .Except(actualesSet)
                .Select(d => new PlatoDiaPreferido
                {
                    PlatoId = id,
                    Dia = d
                })
                .ToList();

            var paraEliminar = actuales
                .Where(x => !diasEnum.Contains(x.Dia))
                .ToList();

            if (paraEliminar.Count > 0)
                _context.PlatosDiasPreferidos.RemoveRange(paraEliminar);

            if (paraAgregar.Count > 0)
                await _context.PlatosDiasPreferidos.AddRangeAsync(paraAgregar);
        }

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
            Nombre = i.Nombre,
            Tipo = i.Tipo.ToString()
        }).OrderBy(i => i.Nombre);
    }

    public async Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente)
    {
        try
        {
            var exists = await _context.Ingredientes.AnyAsync(i => i.Nombre == ingrediente.Nombre);

            if (exists)
                throw new Exception("El ingrediente ya existe");

            var tipoIngrediente = Enum.Parse<TipoIngrediente>(ingrediente.Tipo, true);

            Ingrediente ingredienteEntity = new Ingrediente
            {
                Nombre = ingrediente.Nombre,
                Tipo = tipoIngrediente
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
            Nombre = ingrediente.Nombre,
            Tipo = ingrediente.Tipo.ToString()
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
            var tipoIngrediente = Enum.Parse<TipoIngrediente>(ingrediente.Tipo, true);
            existingIngrediente.Tipo = tipoIngrediente;

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
    private int PickPlatoForDay(List<PlatoPick> allPlatos, HashSet<int> usados, DateOnly fecha)
    {
        var dow = fecha.ToDateTime(TimeOnly.MinValue).DayOfWeek;

        var candidate = allPlatos.FirstOrDefault(p => p.Preferidos.Contains(dow) && !usados.Contains(p.Id));
        if (candidate != null)
        {
            usados.Add(candidate.Id);
            return candidate.Id;
        }

        candidate = allPlatos.FirstOrDefault(p => !usados.Contains(p.Id));
        if (candidate != null)
        {
            usados.Add(candidate.Id);
            return candidate.Id;
        }

        usados.Clear();
        candidate = allPlatos.FirstOrDefault(p => p.Preferidos.Contains(dow));
        if (candidate != null)
        {
            usados.Add(candidate.Id);
            return candidate.Id;
        }

        candidate = allPlatos.First();
        usados.Add(candidate.Id);
        return candidate.Id;
    }

    private static bool IsSundayToThursday(DateOnly d)
    {
        var dow = d.ToDateTime(TimeOnly.MinValue).DayOfWeek;
        return dow == DayOfWeek.Sunday || dow == DayOfWeek.Monday || dow == DayOfWeek.Tuesday
               || dow == DayOfWeek.Wednesday || dow == DayOfWeek.Thursday;
    }
}