namespace SolicitudesService.Events
{
    public class SolicitudAtendidaEvent
    {
        public Guid EventId { get; set; }

        public int SolicitudId { get; set; }

        public int MovimientoId { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}