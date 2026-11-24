namespace SolicitudesService.Events
{
    public class SolicitudAprobadaEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();

        public int SolicitudId { get; set; }

        public int BodegaId { get; set; }

        public string UsuarioAprobadorId { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public List<SolicitudAprobadaDetalleEvent> Detalles { get; set; }
            = new();
    }

    public class SolicitudAprobadaDetalleEvent
    {
        public int MaterialId { get; set; }

        public decimal Cantidad { get; set; }
    }
}