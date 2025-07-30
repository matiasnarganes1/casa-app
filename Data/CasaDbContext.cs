using CasaApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaApp.Api.Data;

public class CasaDbContext : DbContext
{
    public CasaDbContext(DbContextOptions<CasaDbContext> options) : base(options) { }

    public DbSet<Plato> Platos => Set<Plato>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<PlatoIngrediente> PlatoIngredientes => Set<PlatoIngrediente>();

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
            .HasForeignKey(pi => pi.IngredienteId);
    }
}