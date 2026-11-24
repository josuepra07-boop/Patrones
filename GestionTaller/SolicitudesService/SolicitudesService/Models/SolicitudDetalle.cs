namespace SolicitudesService.Models
{
    public class SolicitudDetalle
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public int MaterialId { get; set; }

        public string CodigoMaterial { get; set; } = string.Empty;

        public string DescripcionMaterial { get; set; } = string.Empty;

        public decimal CantidadSolicitada { get; set; }

        public decimal? CantidadAprobada { get; set; }

        public string? Observacion { get; set; }

        public Solicitud? Solicitud { get; set; }
    }
}