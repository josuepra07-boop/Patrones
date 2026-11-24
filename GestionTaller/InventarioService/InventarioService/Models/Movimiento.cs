namespace InventarioService.Models
{
    public class Movimiento
    {
        public int Id { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string UsuarioId { get; set; } = string.Empty;

        public int? BodegaOrigenId { get; set; }

        public int? BodegaDestinoId { get; set; }

        public int? SolicitudId { get; set; }

        public string? Observacion { get; set; }

        public ICollection<MovimientoDetalle> Detalles { get; set; }
            = new List<MovimientoDetalle>();
    }
}