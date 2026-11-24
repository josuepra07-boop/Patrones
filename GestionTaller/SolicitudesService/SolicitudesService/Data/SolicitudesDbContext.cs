using Microsoft.EntityFrameworkCore;
using SolicitudesService.Models;

namespace SolicitudesService.Data
{
    public class SolicitudesDbContext : DbContext
    {
        public SolicitudesDbContext(
            DbContextOptions<SolicitudesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Solicitud> Solicitudes { get; set; }

        public DbSet<SolicitudDetalle> SolicitudDetalles { get; set; }

        public DbSet<SolicitudHistorial> SolicitudHistorial { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitudDetalle>()
                .Property(x => x.CantidadSolicitada)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SolicitudDetalle>()
                .Property(x => x.CantidadAprobada)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Solicitud>()
                .HasMany(x => x.Detalles)
                .WithOne(x => x.Solicitud)
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}