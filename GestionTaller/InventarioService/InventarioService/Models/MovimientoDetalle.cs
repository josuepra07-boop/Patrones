namespace InventarioService.Models
{
    public class MovimientoDetalle
    {
        public int Id { get; set; }

        public int MovimientoId { get; set; }

        public int MaterialId { get; set; }

        public decimal Cantidad { get; set; }

        public Movimiento? Movimiento { get; set; }

        public Material? Material { get; set; }
    }
}