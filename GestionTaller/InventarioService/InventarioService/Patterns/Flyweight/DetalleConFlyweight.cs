namespace InventarioService.Patterns.Flyweight
{
    /// <summary>
    /// Contexto del patrón Flyweight.
    /// Material contiene el estado intrínseco compartido.
    /// El resto de propiedades corresponde al estado extrínseco del movimiento.
    /// </summary>
    public sealed class DetalleConFlyweight
    {
        public MaterialFlyweight Material { get; set; } = null!;

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
