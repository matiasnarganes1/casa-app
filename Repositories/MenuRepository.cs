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

    public async Task<Menu?> CreateMenuAsync(CreateMenuDto menu)
    {
        if (menu == null) throw new ArgumentNullException(nameof(menu));

        var platosMeta = await _context.Platos
            .Select(p => new
            {
                p.Id,
                p.Preferencia,
                p.OneShot,
                Dias = p.DiasPreferidos.Select(d => d.Dia)
            })
            .OrderByDescending(x => x.Preferencia)
            .ToListAsync();

        if (platosMeta.Count == 0) throw new InvalidOperationException("No hay platos disponibles para armar el menú.");

        var allPlatos = platosMeta
            .Select(p => new PlatoPick
            {
                Id = p.Id,
                Preferencia = p.Preferencia,
                OneShot = p.OneShot,
                Preferidos = new HashSet<DayOfWeek>(p.Dias.Select(DiaSemanaHelper.MapDiaSemanaToDayOfWeek))
            })
            .ToList();

        var menuEntity = new Menu
        {
            Nombre = menu.Nombre.Trim(),
            FechaInicio = menu.FechaInicio,
            FechaFin = menu.FechaFin
        };

        _context.Menus.Add(menuEntity);
        await _context.SaveChangesAsync();

        var usadosPorDia = new Dictionary<DayOfWeek, HashSet<int>>();
        var usadosGlobal = new HashSet<int>();
        var carryAlmuerzo = new Dictionary<DateOnly, int>();

        var orderedAllMoments = new[] { MomentoDelDia.Desayuno, MomentoDelDia.Almuerzo, MomentoDelDia.Merienda, MomentoDelDia.Cena };
        var momentosHabilitados = orderedAllMoments.Where(m => menu.Momentos.Contains(m)).ToArray();

        var timeline = new List<(DateOnly dia, MomentoDelDia momento)>();
        if (momentosHabilitados.Contains(MomentoDelDia.Cena))
            timeline.Add((menu.FechaInicio, MomentoDelDia.Cena));
        for (var d = menu.FechaInicio.AddDays(1); d <= menu.FechaFin; d = d.AddDays(1))
            foreach (var m in momentosHabilitados)
                timeline.Add((d, m));

        var rows = new List<PlatoMenu>();

        foreach (var (dia, momento) in timeline)
        {
            var dow = dia.ToDateTime(TimeOnly.MinValue).DayOfWeek;

            if (momento == MomentoDelDia.Almuerzo && carryAlmuerzo.TryGetValue(dia, out var platoFromCena))
            {
                rows.Add(new PlatoMenu { MenuId = menuEntity.Id, PlatoId = platoFromCena, Dia = dia, Momento = MomentoDelDia.Almuerzo });
                carryAlmuerzo.Remove(dia);
                continue;
            }

            var pick = PickPlatoForDay(allPlatos, usadosPorDia, usadosGlobal, dow);

            rows.Add(new PlatoMenu { MenuId = menuEntity.Id, PlatoId = pick.Id, Dia = dia, Momento = momento });

            if (momento == MomentoDelDia.Cena && !pick.OneShot)
            {
                var next = dia.AddDays(1);
                if (IsSundayToThursday(dia) && next <= menu.FechaFin && momentosHabilitados.Contains(MomentoDelDia.Almuerzo))
                    carryAlmuerzo[next] = pick.Id;
            }
        }

        await _context.PlatoMenus.AddRangeAsync(rows);
        await _context.SaveChangesAsync();

        var menuCreated = await GetMenuByNameAsync(menu.Nombre);
        return menuCreated;
    }

    public async Task<MenuDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus
            .AsNoTracking()
            .Include(m => m.ListaDeCompras)
                .ThenInclude(l => l!.Items)
                    .ThenInclude(i => i.Ingrediente)
            .Include(m => m.Platos)
                .ThenInclude(pm => pm.Plato)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null) return null;

        return new MenuDto
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
            }).ToList(),
            ListaDeCompras = menu.ListaDeCompras == null
                ? new ListaDeComprasDto()
                : new ListaDeComprasDto
                {
                    Id = menu.ListaDeCompras.Id,
                    MenuId = menu.ListaDeCompras.MenuId,
                    Items = menu.ListaDeCompras.Items.Select(i => new ListaDeComprasItemDto
                    {
                        IngredienteId = i.IngredienteId,
                        IngredienteNombre = i.Ingrediente.Nombre,
                        CantidadTotal = i.CantidadTotal,
                        UnidadMedida = i.UnidadMedida
                    }).ToList()
                }
        };
    }

    public async Task<Menu?> GetMenuByNameAsync(string name)
    {
        return await _context.Menus
            .Include(m => m.ListaDeCompras)
                .ThenInclude(l => l!.Items)
                    .ThenInclude(i => i.Ingrediente)
            .Include(m => m.Platos)
                .ThenInclude(p => p.Plato)
                    .ThenInclude(p => p.Ingredientes)
                        .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(m => m.Nombre == name);
    }

    public async Task<List<MenuDto>> GetAllAsync()
    {
        var menuList = await _context.Menus
            .AsNoTracking()
            .Include(m => m.ListaDeCompras)
                .ThenInclude(l => l!.Items)
                    .ThenInclude(i => i.Ingrediente)
            .Include(m => m.Platos)
                .ThenInclude(pm => pm.Plato)
            .OrderBy(m => m.FechaInicio)
            .ToListAsync();

        var result = new List<MenuDto>();
        foreach (var menu in menuList)
        {
            result.Add(new MenuDto
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
                }).ToList(),
                ListaDeCompras = menu.ListaDeCompras == null
                    ? new ListaDeComprasDto()
                    : new ListaDeComprasDto
                    {
                        Id = menu.ListaDeCompras.Id,
                        MenuId = menu.ListaDeCompras.MenuId,
                        Items = menu.ListaDeCompras.Items.Select(i => new ListaDeComprasItemDto
                        {
                            IngredienteId = i.IngredienteId,
                            IngredienteNombre = i.Ingrediente.Nombre,
                            CantidadTotal = i.CantidadTotal,
                            UnidadMedida = i.UnidadMedida
                        }).ToList()
                    }
            });
        }
        return result;
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
        var platos = await _context.Platos
            .AsNoTracking()
            .Include(d => d.DiasPreferidos)
            .Include(p => p.Ingredientes)
                .ThenInclude(i => i.Ingrediente)
            .OrderByDescending(p => p.Preferencia)
            .ToListAsync();

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
        var platoEntity = new Plato
        {
            Nombre = plato.Nombre.Trim(),
            Preferencia = plato.Preferencia,
            OneShot = plato.OneShot
        };

        _context.Platos.Add(platoEntity);
        await _context.SaveChangesAsync();

        var diasEnum = (plato.DiasPreferidos ?? new())
            .Select(DiaSemanaHelper.Parse)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (plato.Ingredientes is not null && plato.Ingredientes.Count > 0)
        {
            var nuevos = new List<PlatoIngrediente>();
            foreach (var ingrediente in plato.Ingredientes)
            {
                var ingredienteEntity = await _context.Ingredientes.FindAsync(ingrediente.Id);
                if (ingredienteEntity == null) throw new InvalidOperationException($"El ingrediente con ID {ingrediente.Id} no existe.");
                nuevos.Add(new PlatoIngrediente
                {
                    PlatoId = platoEntity.Id,
                    IngredienteId = ingredienteEntity.Id,
                    Cantidad = ingrediente.Cantidad,
                    UnidadMedida = ingrediente.UnidadMedida
                });
            }
            if (nuevos.Count > 0) await _context.PlatoIngredientes.AddRangeAsync(nuevos);
        }

        if (diasEnum.Count > 0)
        {
            var rowsDia = diasEnum.Select(d => new PlatoDiaPreferido
            {
                PlatoId = platoEntity.Id,
                Dia = d
            });
            await _context.PlatosDiasPreferidos.AddRangeAsync(rowsDia);
        }

        await _context.SaveChangesAsync();

        return new PlatoDto
        {
            Id = platoEntity.Id,
            Nombre = platoEntity.Nombre,
            Preferencia = platoEntity.Preferencia,
            OneShot = platoEntity.OneShot
        };
    }

    public async Task<PlatoDto?> GetPlatoWithIngredientesAsync(int id)
    {
        var plato = await _context.Platos
            .AsNoTracking()
            .Include(p => p.Ingredientes)
                .ThenInclude(pi => pi.Ingrediente)
            .Include(p => p.DiasPreferidos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plato == null) return null;

        return new PlatoDto
        {
            Id = plato.Id,
            Nombre = plato.Nombre,
            Preferencia = plato.Preferencia,
            OneShot = plato.OneShot,
            Ingredientes = plato.Ingredientes.Select(pi => new IngredienteEnPlatoDto
            {
                Id = pi.Ingrediente!.Id,
                Nombre = pi.Ingrediente!.Nombre,
                Cantidad = pi.Cantidad,
                UnidadMedida = pi.UnidadMedida
            }).ToList(),
            DiasPreferidos = plato.DiasPreferidos
                .OrderBy(d => d.Dia)
                .Select(d => d.Dia.ToStringLabel())
                .ToList()
        };
    }

    public async Task<Plato?> GetPlatoWithEntities(int id)
    {
        return await _context.Platos
            .Include(d => d.DiasPreferidos)
            .Include(p => p.Ingredientes)
                .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Plato?> GetPlatoByName(string name)
    {
        return await _context.Platos.AsNoTracking().FirstOrDefaultAsync(p => p.Nombre == name);
    }

    public async Task<bool> AddIngredienteToPlatoAsync(int platoId, CreateIngredienteEnPlatoDto ingrediente)
    {
        var entity = new PlatoIngrediente
        {
            PlatoId = platoId,
            IngredienteId = ingrediente.IngredienteId,
            Cantidad = ingrediente.Cantidad,
            UnidadMedida = ingrediente.UnidadMedida
        };
        _context.PlatoIngredientes.Add(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePlatoAsync(int id, PlatoDto dto)
    {
        var plato = await _context.Platos
            .Include(p => p.Ingredientes)
            .Include(p => p.DiasPreferidos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plato == null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) plato.Nombre = dto.Nombre.Trim();
        if (dto.Preferencia >= 0) plato.Preferencia = dto.Preferencia;
        plato.OneShot = dto.OneShot;

        if (dto.Ingredientes != null)
        {
            var existentes = await _context.PlatoIngredientes.Where(pi => pi.PlatoId == id).ToListAsync();
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

            var actuales = await _context.PlatosDiasPreferidos.Where(x => x.PlatoId == id).ToListAsync();
            var actualesSet = actuales.Select(x => x.Dia).ToHashSet();

            var paraAgregar = diasEnum
                .Except(actualesSet)
                .Select(d => new PlatoDiaPreferido { PlatoId = id, Dia = d })
                .ToList();

            var paraEliminar = actuales.Where(x => !diasEnum.Contains(x.Dia)).ToList();

            if (paraEliminar.Count > 0) _context.PlatosDiasPreferidos.RemoveRange(paraEliminar);
            if (paraAgregar.Count > 0) await _context.PlatosDiasPreferidos.AddRangeAsync(paraAgregar);
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
        var ingredientes = await _context.Ingredientes.AsNoTracking().OrderBy(i => i.Nombre).ToListAsync();
        return ingredientes.Select(i => new IngredienteDto
        {
            Id = i.Id,
            Nombre = i.Nombre,
            Tipo = i.Tipo.ToString()
        });
    }

    public async Task<bool> CreateIngredienteAsync(CreateIngredienteDto ingrediente)
    {
        var exists = await _context.Ingredientes.AnyAsync(i => i.Nombre == ingrediente.Nombre);
        if (exists) throw new InvalidOperationException("El ingrediente ya existe.");

        var tipoIngrediente = Enum.Parse<TipoIngrediente>(ingrediente.Tipo, true);

        var entity = new Ingrediente
        {
            Nombre = ingrediente.Nombre.Trim(),
            Tipo = tipoIngrediente
        };
        _context.Ingredientes.Add(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IngredienteDto?> GetIngredienteAsync(int id)
    {
        var ingrediente = await _context.Ingredientes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (ingrediente == null) return null;
        return new IngredienteDto
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre,
            Tipo = ingrediente.Tipo.ToString()
        };
    }

    public async Task<IngredienteDto?> GetIngredienteByNameAsync(string nombre)
    {
        var ingrediente = await _context.Ingredientes.AsNoTracking().FirstOrDefaultAsync(p => p.Nombre == nombre);
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
        var existing = await _context.Ingredientes.FindAsync(id);
        if (existing == null) return false;

        var exists = await _context.Ingredientes.AnyAsync(i => i.Id != id && i.Nombre == ingrediente.Nombre);
        if (exists) throw new InvalidOperationException("El ingrediente ya existe.");

        existing.Nombre = ingrediente.Nombre.Trim();
        existing.Tipo = Enum.Parse<TipoIngrediente>(ingrediente.Tipo, true);

        _context.Ingredientes.Update(existing);
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

    public async Task<bool> DeleteIngredienteFromPlatoAsync(int platoId, int ingredienteId)
    {
        var platoIngrediente = await _context.PlatoIngredientes
            .FirstOrDefaultAsync(pi => pi.PlatoId == platoId && pi.IngredienteId == ingredienteId);
        if (platoIngrediente == null) return false;

        _context.PlatoIngredientes.Remove(platoIngrediente);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Menu?> GetMenuWithPlatosAndIngredientesAsync(int menuId)
    {
        return await _context.Menus
            .Include(m => m.Platos)
                .ThenInclude(pm => pm.Plato)
                    .ThenInclude(p => p.Ingredientes)
                        .ThenInclude(pi => pi.Ingrediente)
            .FirstOrDefaultAsync(m => m.Id == menuId);
    }

    public async Task UpsertShoppingListAsync(int menuId, IEnumerable<ListaDeComprasItem> items)
    {
        var existing = await _context.ListasDeCompras
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.MenuId == menuId);

        if (existing == null)
        {
            var list = new ListaDeCompras { MenuId = menuId, Items = items.ToList() };
            _context.ListasDeCompras.Add(list);
        }
        else
        {
            _context.ListasDeComprasItems.RemoveRange(existing.Items);
            foreach (var it in items) existing.Items.Add(it);
        }

        await _context.SaveChangesAsync();
    }

    private sealed class PlatoPick
    {
        public int Id { get; set; }
        public int Preferencia { get; set; }
        public bool OneShot { get; set; }
        public HashSet<DayOfWeek> Preferidos { get; set; } = new();
    }

    private PlatoPick PickPlatoForDay(
    List<PlatoPick> allPlatos,
    Dictionary<DayOfWeek, HashSet<int>> usadosPorDia,
    HashSet<int> usadosGlobal,
    DayOfWeek dow)
    {
        if (!usadosPorDia.TryGetValue(dow, out var usadosDia))
        {
            usadosDia = new HashSet<int>();
            usadosPorDia[dow] = usadosDia;
        }

        var preferidos = allPlatos.Where(p => p.Preferidos.Contains(dow)).ToList();
        var candidate = preferidos.FirstOrDefault(p => !usadosGlobal.Contains(p.Id));
        if (candidate != null)
        {
            usadosGlobal.Add(candidate.Id);
            usadosDia.Add(candidate.Id);
            return candidate;
        }

        candidate = allPlatos.FirstOrDefault(p => !usadosGlobal.Contains(p.Id));
        if (candidate != null)
        {
            usadosGlobal.Add(candidate.Id);
            return candidate;
        }

        usadosGlobal.Clear();

        candidate = preferidos.FirstOrDefault();
        if (candidate != null)
        {
            usadosGlobal.Add(candidate.Id);
            usadosDia.Add(candidate.Id);
            return candidate;
        }

        candidate = allPlatos.First();
        usadosGlobal.Add(candidate.Id);
        return candidate;
    }

    private static bool IsSundayToThursday(DateOnly d)
    {
        var dow = d.ToDateTime(TimeOnly.MinValue).DayOfWeek;
        return dow == DayOfWeek.Sunday || dow == DayOfWeek.Monday || dow == DayOfWeek.Tuesday
               || dow == DayOfWeek.Wednesday || dow == DayOfWeek.Thursday;
    }
}