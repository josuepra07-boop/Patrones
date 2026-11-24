namespace InventarioService.Events
{
    public class SolicitudAtendidaEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();

        public int SolicitudId { get; set; }

        public int MovimientoId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "ATENDIDA";
    }
}