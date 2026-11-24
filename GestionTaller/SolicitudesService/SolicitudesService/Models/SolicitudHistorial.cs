namespace SolicitudesService.Models
{
    public class SolicitudHistorial
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public string? EstadoAnterior { get; set; }

        public string EstadoNuevo { get; set; } = string.Empty;

        public string UsuarioId { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string? Observacion { get; set; }

        public Solicitud? Solicitud { get; set; }
    }
}