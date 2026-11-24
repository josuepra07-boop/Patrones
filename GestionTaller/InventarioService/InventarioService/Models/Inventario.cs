namespace InventarioService.Models
{
    public class Inventario
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }

        public int BodegaId { get; set; }

        public decimal CantidadDisponible { get; set; }

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public Material? Material { get; set; }

        public Bodega? Bodega { get; set; }
    }
}