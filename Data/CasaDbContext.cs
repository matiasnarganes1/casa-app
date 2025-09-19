using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CasaApp.Api.Models;

namespace CasaApp.Api.Data;

public partial class CasaDbContext : DbContext
{
    public CasaDbContext(DbContextOptions<CasaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<ListaDeCompras> ListaDeCompras { get; set; }

    public virtual DbSet<ListaDeComprasItem> ListaDeComprasItems { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }

    public virtual DbSet<PlatoIngrediente> PlatoIngredientes { get; set; }

    public virtual DbSet<PlatoMenu> PlatoMenus { get; set; }

    public virtual DbSet<PlatoDiaPreferido> PlatosDiasPreferidos { get; set; }

    //public virtual DbSet<__EFMigrationsHistory> __EFMigrationsHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Nombre, "IX_Ingredientes_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<ListaDeCompras>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.MenuId, "IX_ListaDeCompras_MenuId").IsUnique();

            entity.HasOne(d => d.Menu).WithOne(p => p.ListaDeCompras).HasForeignKey<ListaDeCompras>(d => d.MenuId);
        });

        modelBuilder.Entity<ListaDeComprasItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ListaDeComprasItem");

            entity.HasIndex(e => e.IngredienteId, "IX_ListaDeComprasItem_IngredienteId");

            entity.HasIndex(e => new { e.ListaDeComprasId, e.IngredienteId, e.UnidadMedida }, "IX_ListaDeComprasItem_ListaDeComprasId_IngredienteId_UnidadMedi~").IsUnique();

            entity.Property(e => e.UnidadMedida).HasMaxLength(50);

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.ListaDeComprasItems).HasForeignKey(d => d.IngredienteId);

            entity.HasOne(d => d.ListaDeCompras).WithMany(p => p.Items).HasForeignKey(d => d.ListaDeComprasId);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Menu");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Nombre, "IX_Platos_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<PlatoIngrediente>(entity =>
        {
            entity.HasKey(e => new { e.PlatoId, e.IngredienteId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasIndex(e => e.IngredienteId, "IX_PlatoIngredientes_IngredienteId");

            entity.HasIndex(e => e.PlatoId, "IX_PlatoIngredientes_PlatoId");

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.PlatoIngredientes)
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Plato).WithMany(p => p.Ingredientes).HasForeignKey(d => d.PlatoId);
        });

        modelBuilder.Entity<PlatoMenu>(entity =>
        {
            entity.HasKey(e => new { e.MenuId, e.PlatoId, e.Dia, e.Momento })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity.ToTable("PlatoMenu");

            entity.HasIndex(e => e.MenuId, "IX_PlatoMenu_MenuId");

            entity.HasIndex(e => new { e.MenuId, e.Dia, e.Momento }, "IX_PlatoMenu_MenuId_Dia_Momento").IsUnique();

            entity.HasIndex(e => e.PlatoId, "IX_PlatoMenu_PlatoId");

            entity.HasOne(d => d.Menu).WithMany(p => p.Platos).HasForeignKey(d => d.MenuId);

            entity.HasOne(d => d.Plato).WithMany(p => p.PlatoMenus).HasForeignKey(d => d.PlatoId);
        });

        modelBuilder.Entity<PlatoDiaPreferido>(entity =>
        {
            entity.HasKey(e => new { e.PlatoId, e.Dia })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.Plato).WithMany(p => p.DiasPreferidos).HasForeignKey(d => d.PlatoId);
        });

        // modelBuilder.Entity<__EFMigrationsHistory>(entity =>
        // {
        //     entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

        //     entity.ToTable("__EFMigrationsHistory");

        //     entity.Property(e => e.MigrationId).HasMaxLength(150);
        //     entity.Property(e => e.ProductVersion).HasMaxLength(32);
        // });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
