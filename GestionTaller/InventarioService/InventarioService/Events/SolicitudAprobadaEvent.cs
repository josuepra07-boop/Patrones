namespace InventarioService.Events
{
    public class SolicitudAprobadaEvent
    {
        public Guid EventId { get; set; }

        public int SolicitudId { get; set; }

        public int BodegaId { get; set; }

        public string UsuarioAprobadorId { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public List<SolicitudAprobadaDetalleEvent> Detalles { get; set; }
            = new();
    }

    public class SolicitudAprobadaDetalleEvent
    {
        public int MaterialId { get; set; }

        public decimal Cantidad { get; set; }
    }
}