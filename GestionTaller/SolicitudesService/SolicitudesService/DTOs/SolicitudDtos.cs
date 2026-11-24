namespace SolicitudesService.DTOs
{
    public class CrearSolicitudDto
    {
        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public List<CrearSolicitudDetalleDto> Detalles { get; set; }
            = new();
    }

    public class CrearSolicitudDetalleDto
    {
        public int MaterialId { get; set; }

        public string CodigoMaterial { get; set; } = string.Empty;

        public string DescripcionMaterial { get; set; } = string.Empty;

        public decimal Cantidad { get; set; }

        public string? Observacion { get; set; }
    }

    public class AprobarSolicitudDto
    {
        public string UsuarioAprobadorId { get; set; } = string.Empty;
        public int BodegaId { get; set; }

        public string? Observacion { get; set; }

        public List<AprobarSolicitudDetalleDto> Detalles { get; set; }
            = new();
    }

    public class AprobarSolicitudDetalleDto
    {
        public int DetalleId { get; set; }

        public decimal CantidadAprobada { get; set; }
    }

    public class RechazarSolicitudDto
    {
        public string UsuarioId { get; set; } = string.Empty;

        public string? Observacion { get; set; }
    }

    public class ClonarSolicitudDto
    {
        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        public string? Observacion { get; set; }
    }

    public class SolicitudDto
    {
        public int Id { get; set; }

        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public string? UsuarioAprobadorId { get; set; }

        public List<SolicitudDetalleDto> Detalles { get; set; }
            = new();

    }

    public class SolicitudDetalleDto
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }

        public string CodigoMaterial { get; set; } = string.Empty;

        public string DescripcionMaterial { get; set; } = string.Empty;

        public decimal CantidadSolicitada { get; set; }

        public decimal? CantidadAprobada { get; set; }

        public string? Observacion { get; set; }
    }

    
}