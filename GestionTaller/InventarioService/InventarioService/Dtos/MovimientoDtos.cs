namespace InventarioService.DTOs
{
    public class InventarioDto
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }

        public string CodigoMaterial { get; set; } = string.Empty;

        public string DescripcionMaterial { get; set; } = string.Empty;

        public int BodegaId { get; set; }

        public string Bodega { get; set; } = string.Empty;

        public decimal CantidadDisponible { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }

    public class MovimientoDetalleRequestDto
    {
        public int MaterialId { get; set; }

        public decimal Cantidad { get; set; }
    }

    public class EntradaDto
    {
        public int BodegaDestinoId { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public List<MovimientoDetalleRequestDto> Detalles { get; set; }
            = new();
    }

    public class SalidaDto
    {
        public int BodegaOrigenId { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public int? SolicitudId { get; set; }

        public string? Observacion { get; set; }

        public List<MovimientoDetalleRequestDto> Detalles { get; set; }
            = new();
    }

    public class TrasladoDto
    {
        public int BodegaOrigenId { get; set; }

        public int BodegaDestinoId { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public List<MovimientoDetalleRequestDto> Detalles { get; set; }
            = new();
    }

    public class MovimientoResultadoDto
    {
        public int MovimientoId { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }

    public class DevolucionDto
    {
        public int BodegaDestinoId { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public int? SolicitudId { get; set; }

        public string? Observacion { get; set; }

        public List<MovimientoDetalleRequestDto> Detalles { get; set; }
            = new();
    }

    public class AjusteDto
    {
        public int BodegaId { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        // POSITIVO o NEGATIVO
        public string TipoAjuste { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public List<MovimientoDetalleRequestDto> Detalles { get; set; }
            = new();
    }

    public class MovimientoConsultaDto
    {
        public int Id { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public int? BodegaOrigenId { get; set; }

        public int? BodegaDestinoId { get; set; }

        public int? SolicitudId { get; set; }

        public string? Observacion { get; set; }

        public List<MovimientoDetalleConsultaDto> Detalles { get; set; }
            = new();
    }

    public class MovimientoDetalleConsultaDto
    {
        public int MaterialId { get; set; }

        public string CodigoMaterial { get; set; } = string.Empty;

        public string DescripcionMaterial { get; set; } = string.Empty;

        public decimal Cantidad { get; set; }
    }
}