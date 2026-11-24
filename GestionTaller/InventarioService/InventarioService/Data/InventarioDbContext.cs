using InventarioService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Data
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<MovimientoDetalle> MovimientoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CATEGORIA
            modelBuilder.Entity<Categoria>()
                .HasIndex(x => x.Nombre)
                .IsUnique();

            // MATERIAL
            modelBuilder.Entity<Material>()
                .HasIndex(x => x.Codigo)
                .IsUnique();

            modelBuilder.Entity<Material>()
                .Property(x => x.StockMinimo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Material>()
                .HasOne(x => x.Categoria)
                .WithMany(x => x.Materiales)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // BODEGA
            modelBuilder.Entity<Bodega>()
                .HasIndex(x => x.Codigo)
                .IsUnique();

            // INVENTARIO
            modelBuilder.Entity<Inventario>()
                .Property(x => x.CantidadDisponible)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Inventario>()
                .HasIndex(x => new { x.MaterialId, x.BodegaId })
                .IsUnique();

            modelBuilder.Entity<Inventario>()
                .HasOne(x => x.Material)
                .WithMany(x => x.Inventarios)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventario>()
                .HasOne(x => x.Bodega)
                .WithMany(x => x.Inventarios)
                .HasForeignKey(x => x.BodegaId)
                .OnDelete(DeleteBehavior.Restrict);

            // MOVIMIENTO DETALLE
            modelBuilder.Entity<MovimientoDetalle>()
                .Property(x => x.Cantidad)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovimientoDetalle>()
                .HasOne(x => x.Movimiento)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.MovimientoId);

            modelBuilder.Entity<MovimientoDetalle>()
                .HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}