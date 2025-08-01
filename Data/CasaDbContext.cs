using CasaApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaApp.Api.Data;

public class CasaDbContext : DbContext
{
    public CasaDbContext(DbContextOptions<CasaDbContext> options) : base(options) { }

    public DbSet<Plato> Platos => Set<Plato>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<PlatoIngrediente> PlatoIngredientes => Set<PlatoIngrediente>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<PlatoMenu> PlatoMenus => Set<PlatoMenu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlatoIngrediente>()
            .HasKey(pi => new { pi.PlatoId, pi.IngredienteId });

        modelBuilder.Entity<PlatoIngrediente>()
            .HasOne(pi => pi.Plato)
            .WithMany(p => p.Ingredientes)
            .HasForeignKey(pi => pi.PlatoId);

        modelBuilder.Entity<PlatoIngrediente>()
            .HasOne(pi => pi.Ingrediente)
            .WithMany(i => i.Platos)
            .HasForeignKey(pi => pi.IngredienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlatoIngrediente>()
            .HasKey(pi => new { pi.PlatoId, pi.IngredienteId });

        modelBuilder.Entity<Menu>().ToTable("Menu");

        modelBuilder.Entity<PlatoMenu>().ToTable("PlatoMenu");

        modelBuilder.Entity<PlatoMenu>()
            .HasKey(pm => new { pm.MenuId, pm.PlatoId, pm.Dia, pm.Momento });

        modelBuilder.Entity<PlatoMenu>()
            .HasOne(pm => pm.Menu)
            .WithMany(m => m.Platos)
            .HasForeignKey(pm => pm.MenuId);

        modelBuilder.Entity<PlatoMenu>()
            .HasOne(pm => pm.Plato)
            .WithMany()
            .HasForeignKey(pm => pm.PlatoId);
    }
}