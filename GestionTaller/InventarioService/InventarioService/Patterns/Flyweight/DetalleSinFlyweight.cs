namespace InventarioService.Patterns.Flyweight
{
    /// <summary>
    /// Representación de comparación SIN Flyweight.
    /// Cada detalle contiene su propia instancia de MaterialSinFlyweight.
    /// </summary>
    public sealed class DetalleSinFlyweight
    {
        public MaterialSinFlyweight Material { get; set; } = null!;

        public int MovimientoId { get; set; }
        public int MovimientoDetalleId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public int? BodegaOrigenId { get; set; }
        public int? BodegaDestinoId { get; set; }
        public int? SolicitudId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
